//-----------------------------KINDLY IGNORE THIS----------------------------------------------------------------------------------------------
// using Microsoft.Extensions.Configuration;
// using Microsoft.IdentityModel.Tokens;
// using System;
// using System.IdentityModel.Tokens.Jwt;
// using System.Text;

// namespace SafeVaultAPI.Utilities
// {
//     internal class JwtManager : IJwtManager
//     {
//         private readonly List<ClientSettings> _clientSettings;

//         public JwtManager(List<ClientSettings> clientSettings)
//         {
//             _clientSettings = clientSettings;
//         }

//         public string GenerateToken(string clientName)
//         {
//             string result = string.Empty;

//             var clientSettings = _clientSettings.FirstOrDefault(c => c.Client == clientName);

//             if (clientSettings == null)
//             {
//                 throw new Exception("Client not found.");
//             }

//             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientSettings.Key));
//             var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//             var token = new JwtSecurityToken(
//                 issuer: clientSettings.Issuer,
//                 audience: clientSettings.Audience,
//                 expires: DateTime.UtcNow.AddMinutes(clientSettings.DurationInMinutes),
//                 signingCredentials: creds
//             );

//             return result;
//         }

//         public bool ValidateToken(string token)
//         {
//             bool result = false;

//             return result;
//         }
//     }
// }
//-------------------------------------------------------------------------------------------------------------------------------------------------