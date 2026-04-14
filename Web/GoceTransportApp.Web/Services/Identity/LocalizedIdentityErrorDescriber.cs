namespace GoceTransportApp.Web.Services.Identity
{
    using GoceTransportApp.Web.Resources;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Localization;

    public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
    {
        private readonly IStringLocalizer<SharedResource> localizer;

        public LocalizedIdentityErrorDescriber(IStringLocalizer<SharedResource> localizer)
        {
            this.localizer = localizer;
        }

        public override IdentityError DefaultError() => new()
        {
            Code = nameof(DefaultError),
            Description = localizer["DefaultError"],
        };

        public override IdentityError ConcurrencyFailure() => new()
        {
            Code = nameof(ConcurrencyFailure),
            Description = localizer["ConcurrencyFailure"],
        };

        public override IdentityError PasswordMismatch() => new()
        {
            Code = nameof(PasswordMismatch),
            Description = localizer["PasswordMismatch"],
        };

        public override IdentityError InvalidToken() => new()
        {
            Code = nameof(InvalidToken),
            Description = localizer["InvalidToken"],
        };

        public override IdentityError RecoveryCodeRedemptionFailed() => new()
        {
            Code = nameof(RecoveryCodeRedemptionFailed),
            Description = localizer["RecoveryCodeRedemptionFailed"],
        };

        public override IdentityError LoginAlreadyAssociated() => new()
        {
            Code = nameof(LoginAlreadyAssociated),
            Description = localizer["LoginAlreadyAssociated"],
        };

        public override IdentityError InvalidUserName(string userName) => new()
        {
            Code = nameof(InvalidUserName),
            Description = localizer["InvalidUserName", userName ?? string.Empty],
        };

        public override IdentityError InvalidEmail(string email) => new()
        {
            Code = nameof(InvalidEmail),
            Description = localizer["InvalidEmail", email ?? string.Empty],
        };

        public override IdentityError DuplicateUserName(string userName) => new()
        {
            Code = nameof(DuplicateUserName),
            Description = localizer["DuplicateUserName", userName ?? string.Empty],
        };

        public override IdentityError DuplicateEmail(string email) => new()
        {
            Code = nameof(DuplicateEmail),
            Description = localizer["DuplicateEmail", email ?? string.Empty],
        };

        public override IdentityError InvalidRoleName(string role) => new()
        {
            Code = nameof(InvalidRoleName),
            Description = localizer["InvalidRoleName", role ?? string.Empty],
        };

        public override IdentityError DuplicateRoleName(string role) => new()
        {
            Code = nameof(DuplicateRoleName),
            Description = localizer["DuplicateRoleName", role ?? string.Empty],
        };

        public override IdentityError UserAlreadyHasPassword() => new()
        {
            Code = nameof(UserAlreadyHasPassword),
            Description = localizer["UserAlreadyHasPassword"],
        };

        public override IdentityError UserLockoutNotEnabled() => new()
        {
            Code = nameof(UserLockoutNotEnabled),
            Description = localizer["UserLockoutNotEnabled"],
        };

        public override IdentityError UserAlreadyInRole(string role) => new()
        {
            Code = nameof(UserAlreadyInRole),
            Description = localizer["UserAlreadyInRole", role ?? string.Empty],
        };

        public override IdentityError UserNotInRole(string role) => new()
        {
            Code = nameof(UserNotInRole),
            Description = localizer["UserNotInRole", role ?? string.Empty],
        };

        public override IdentityError PasswordTooShort(int length) => new()
        {
            Code = nameof(PasswordTooShort),
            Description = localizer["PasswordTooShort", length],
        };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new()
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = localizer["PasswordRequiresUniqueChars", uniqueChars],
        };

        public override IdentityError PasswordRequiresNonAlphanumeric() => new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = localizer["PasswordRequiresNonAlphanumeric"],
        };

        public override IdentityError PasswordRequiresDigit() => new()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = localizer["PasswordRequiresDigit"],
        };

        public override IdentityError PasswordRequiresLower() => new()
        {
            Code = nameof(PasswordRequiresLower),
            Description = localizer["PasswordRequiresLower"],
        };

        public override IdentityError PasswordRequiresUpper() => new()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = localizer["PasswordRequiresUpper"],
        };
    }
}
