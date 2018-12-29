using System.Linq;
using System.Threading.Tasks;
using Discussion.Core.Communication.Sms;
using Discussion.Core.Data;
using Discussion.Core.Models;
using Discussion.Core.Time;
using Discussion.Core.Utilities;

namespace Discussion.Web.Services.UserManagement.PhoneNumberVerification
{
    public class DefaultPhoneNumberVerificationService : IPhoneNumberVerificationService
    {
        private readonly IRepository<PhoneNumberVerificationRecord> _verificationCodeRecordRepo;
        private readonly ISmsSender _smsSender;
        private readonly IClock _clock;

        public DefaultPhoneNumberVerificationService(
            IRepository<PhoneNumberVerificationRecord> verificationCodeRecordRepo, 
            ISmsSender smsSender, IClock clock)
        {
            _verificationCodeRecordRepo = verificationCodeRecordRepo;
            _smsSender = smsSender;
            _clock = clock;
        }

        public bool IsFrequencyExceededForUser(int userId)
        {
            var today = _clock.Now.UtcDateTime.Date;
            var msgSentToday = _verificationCodeRecordRepo
                .All()
                .Where(r => r.UserId == userId && r.CreatedAtUtc >= today)
                .ToList();
            
            var sentInTwoMinutes = msgSentToday.Any(r => r.CreatedAtUtc > _clock.Now.UtcDateTime.AddMinutes(-2));

            return msgSentToday.Count >= 5 || sentInTwoMinutes;
        }

        public async Task SendVerificationCodeAsync(int userId, string phoneNumber)
        {
            var verificationCode = StringUtility.RandomNumbers(length:6);
            
            var record = new PhoneNumberVerificationRecord
            {
                UserId = userId,
                Code = verificationCode,
                Expires = _clock.Now.UtcDateTime.AddMinutes(15),
                PhoneNumber = phoneNumber
            };
            _verificationCodeRecordRepo.Save(record);
            
            await _smsSender.SendVerificationCodeAsync(phoneNumber, verificationCode);
        }

        public string GetVerifiedPhoneNumberByCode(int userId, string providedVerificationCode)
        {
            var validCode = _verificationCodeRecordRepo
                .All()
                .FirstOrDefault(r => r.UserId == userId 
                                     && r.Code == providedVerificationCode 
                                     && r.Expires > _clock.Now.UtcDateTime);
            return validCode?.PhoneNumber;
        }
    }
}