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

                var userDn = $"mail={email},ou=users,dc=mydomain,dc=com";

                var credentials = new NetworkCredential(userDn, password);
                var identifier = new LdapDirectoryIdentifier(_config.Url, _config.Port);


                using (var connection = new System.DirectoryServices.Protocols.LdapConnection(identifier))
                {
                    connection.SessionOptions.ProtocolVersion = 3;
                    connection.Credential = credentials;
                    connection.AuthType = System.DirectoryServices.Protocols.AuthType.Basic;
                    connection.Bind(); 

                    _logger.LogInformation("Authentication successful for user: {Email}", email);

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

                _logger.LogError(ex, "An unspecified error occurred while attempting to authenticate user: {Email}", email);
            }

            return "user";
        }
    }
}
