using Novell.Directory.Ldap;
using System;
using System.DirectoryServices.Protocols;
using Microsoft.Extensions.Logging;
using LdapException = Novell.Directory.Ldap.LdapException;
using System.Net;

namespace Multimedia.Models
{
    public class LdapService
    {
        private LdapConfig _config;
        private readonly ILogger _logger;

        public LdapService(LdapConfig config, ILogger<LdapService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string GetUserRole(string email, string password)
        {
            try
            {
                _logger.LogInformation("Attempting to authenticate user: {Email}", email);

                // DN użytkownika
                var userDn = $"mail={email},ou=users,dc=mydomain,dc=com";

                // Tworzenie poświadczeń
                var credentials = new NetworkCredential(userDn, password);
                var identifier = new LdapDirectoryIdentifier(_config.Url, _config.Port);

                // Uwierzytelnianie użytkownika
                using (var connection = new System.DirectoryServices.Protocols.LdapConnection(identifier))
                {
                    connection.SessionOptions.ProtocolVersion = 3;
                    connection.Credential = credentials;
                    connection.AuthType = System.DirectoryServices.Protocols.AuthType.Basic;
                    connection.Bind(); // Jeśli nie ma wyjątku, uwierzytelnianie się powiodło

                    _logger.LogInformation("Authentication successful for user: {Email}", email);

                    // Zwracanie roli w zależności od uwierzytelnienia
                    return "admin";
                }
            }
            catch (DirectoryOperationException ex)
            {
                _logger.LogError(ex, "A directory operation error occurred while attempting to authenticate user: {Email}", email);
            }
            catch (LdapException ex)
            {
                _logger.LogError(ex, "An LDAP error occurred while attempting to authenticate user: {Email}", email);
            }
            catch (Exception ex)
            {
                // Inny rodzaj błędu
                _logger.LogError(ex, "An unspecified error occurred while attempting to authenticate user: {Email}", email);
            }

            // Jeśli uwierzytelnienie się nie powiodło, zwróć "user"
            return "user";
        }
    }
}
