using ContentFlow.Domain.Enums;

namespace ContentFlow.Domain.Entities;

public class UserProfile
{
    #region Properties

        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string? MiddleName { get; private set; }
        public DateOnly? BirthDate { get; private set; }
        public string? City { get; private set; }
        public string? Bio { get; private set; } // About myself
        public Gender Gender { get; private set; }
        public string? AvatarUrl { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted => DeletedAt.HasValue;
        
    #endregion
    
    #region Constructors

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="userId">Id пользователя (User)</param>
        /// <param name="firstName">Имя</param>
        /// <param name="lastName">Фамилия</param>
        /// <param name="middleName">Отчество</param>
        /// <param name="birthDate">Дата рождения</param>
        /// <param name="city">Адрес/город</param>
        /// <param name="bio">О себе</param>
        /// <param name="avatarUrl">Адрес к картинке профиля</param>
        /// <exception cref="ArgumentException">UserId обязателен для заполнения</exception>
        public UserProfile(
            int userId,
            string? firstName = null,
            string? lastName = null,
            string? middleName = null,
            DateOnly? birthDate = null,
            string? city = null,
            string? bio = null,
            string? avatarUrl = null)
        {
            if (userId <= 0) 
            {
                throw new ArgumentException("UserId must be positive", nameof(userId));
            }
            
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            BirthDate = birthDate;
            City = city;
            Bio = bio;
            AvatarUrl = avatarUrl;
            Gender = Gender.Undefined;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            DeletedAt = null;
        }
        
        /// <summary>
        /// Private constructor
        /// </summary>
        private UserProfile() { }
    
    #endregion
    
    #region Public Methods

        public void MarkDeleted()
        {
            if (DeletedAt.HasValue) return;
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    
        public void UpdateProfile(
            string? firstName,
            string? lastName,
            string? middleName,
            DateOnly? birthDate,
            string? city,
            string? bio,
            Gender gender)
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            BirthDate = birthDate;
            City = city;
            Bio = bio;
            Gender = gender;
            UpdatedAt = DateTime.UtcNow;
        }

        public string ReturnConcatFio()
        {
            return $"{LastName} {FirstName} {MiddleName}";
        }

        public int? CalculateAgeByBirthDate()
        {
            if (!BirthDate.HasValue)
                return null;
            
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var birth = BirthDate.Value;
            
            var age = today.Year - birth.Year;

            if (today < birth.AddYears(age))
                age--;

            return age;
        }

        public void UpdateUserAvatarUri(string? uri)
        {
            AvatarUrl = uri;
            UpdatedAt = DateTime.UtcNow;
        }
    
    #endregion
    
    #region Private Methods
    #endregion
}