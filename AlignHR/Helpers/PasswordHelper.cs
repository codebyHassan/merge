using BCrypt.Net;

namespace AlignHR.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            // BCrypt automatically generates a salt and includes it in the resulting string
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Verifies the password against the stored BCrypt hash
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                // In case an old SHA256 hash is still in the DB, this prevents a crash
                return false;
            }
        }
    }
}