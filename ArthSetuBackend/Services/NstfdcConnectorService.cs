using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArthSetuBackend.Services
{
    public class NstfdcConnectorService : IGovernmentConnector
    {
        private readonly HttpClient _httpClient;
        private readonly SourceSyncService _syncService;
        private readonly string _url = "https://nstfdc.tribal.gov.in"; 

        public NstfdcConnectorService(HttpClient httpClient, SourceSyncService syncService)
        {
            _httpClient = httpClient;
            _syncService = syncService;
        }

        public async Task<Models.SourceSyncLog> RunSyncAsync()
        {
            int httpStatus = 0;
            string payload = "";
            string contentType = "";
            
            try
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "ArthSetu-SourceSync-Bot/1.0");
                _httpClient.Timeout = TimeSpan.FromSeconds(30);

                var response = await _httpClient.GetAsync(_url);
                httpStatus = (int)response.StatusCode;
                
                if (response.IsSuccessStatusCode)
                {
                    payload = await response.Content.ReadAsStringAsync();
                    contentType = response.Content.Headers.ContentType?.ToString() ?? "text/html";
                }
            }
            catch (Exception)
            {
                // NO EMBEDDED FALLBACK - strictly fail closed on error
                return await _syncService.RunSyncTransactionAsync("src-nstfdc", "NstfdcConnectorService", _url, payload, 0, contentType, new List<SchemeStaging>());
            }
            
            var staging = ParseHtml(payload);
            
            return await _syncService.RunSyncTransactionAsync(
                "src-nstfdc",
                "NstfdcConnectorService",
                _url,
                payload,
                httpStatus,
                contentType,
                staging
            );
        }

        private List<SchemeStaging> ParseHtml(string html)
        {
            var results = new List<SchemeStaging>();
            if (string.IsNullOrEmpty(html)) return results;

            // Common NSTFDC attributes
            string targetCommunity = "ST"; 
            decimal defaultIncomeCeiling = 300000; // As per new guidelines in PIB
            
            // Extract from html based on known schemes
            if (html.Contains("Term Loan"))
            {
                results.Add(new SchemeStaging
                {
                    SchemeId = "nstfdc-term",
                    Name = "Term Loan Scheme",
                    Description = ExtractTextNear(html, "Term Loan", 100) ?? "Financial assistance for viable income-generating activities",
                    SchemeCategory = "Term Loan",
                    TargetCommunity = targetCommunity,
                    IncomeCeiling = defaultIncomeCeiling,
                    IncomeCeilingRaw = "3.00 Lakh",
                    LoanMax = ExtractNumericWithLakh(html, @"(?:upto|up to)\s+(?:Rs\.?|₹)", "Term Loan") ?? 2500000,
                    LoanMaxRaw = "25.00 Lakh",
                    InterestRateRaw = "5%", // 5% p.a.
                    SourceSection = "Term Loan Scheme"
                });
            }
            
            if (html.Contains("Adivasi Mahila") || html.Contains("AMSY"))
            {
                results.Add(new SchemeStaging
                {
                    SchemeId = "nstfdc-amsy",
                    Name = "Adivasi Mahila Sashaktikaran Yojana (AMSY)",
                    Description = ExtractTextNear(html, "Adivasi Mahila", 100) ?? "Exclusive scheme for economic empowerment of tribal women",
                    SchemeCategory = "Term Loan",
                    TargetCommunity = targetCommunity,
                    Gender = "Female",
                    IncomeCeiling = defaultIncomeCeiling,
                    IncomeCeilingRaw = "3.00 Lakh",
                    LoanMax = ExtractNumericWithLakh(html, @"(?:upto|up to)\s+(?:Rs\.?|₹)", "Adivasi Mahila") ?? 200000,
                    LoanMaxRaw = "2.00 Lakh",
                    InterestRateRaw = "4%",
                    SourceSection = "Adivasi Mahila Sashaktikaran Yojana"
                });
            }
            
            if (html.Contains("Micro Credit") || html.Contains("SHG"))
            {
                results.Add(new SchemeStaging
                {
                    SchemeId = "nstfdc-mcf",
                    Name = "Micro Credit Finance for SHGs",
                    Description = ExtractTextNear(html, "Micro Credit", 100) ?? "Micro Credit Finance for SHGs",
                    SchemeCategory = "Micro Finance",
                    TargetCommunity = targetCommunity,
                    ApplicantType = "SHG",
                    IncomeCeiling = defaultIncomeCeiling,
                    IncomeCeilingRaw = "3.00 Lakh",
                    LoanMax = ExtractNumericWithLakh(html, @"(?:upto|up to)\s+(?:Rs\.?|₹)", "Micro Credit") ?? 50000,
                    LoanMaxRaw = "50000 per member",
                    SourceSection = "Micro Credit Finance"
                });
            }
            
            if (html.Contains("Adivasi Shiksha Rrinn Yojana") || html.Contains("ASRY"))
            {
                results.Add(new SchemeStaging
                {
                    SchemeId = "nstfdc-asry",
                    Name = "Adivasi Shiksha Rrinn Yojana (ASRY)",
                    Description = "Education loan scheme",
                    SchemeCategory = "Education",
                    TargetCommunity = targetCommunity,
                    IncomeCeiling = defaultIncomeCeiling,
                    IncomeCeilingRaw = "3.00 Lakh",
                    SourceSection = "Adivasi Shiksha Rrinn Yojana"
                });
            }

            return results;
        }

        private string? ExtractTextNear(string text, string keyword, int length)
        {
            int idx = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
            if (idx == -1) return null;
            
            int maxEnd = Math.Min(text.Length - idx, length);
            var substring = text.Substring(idx, maxEnd);
            
            int nextTagIdx = substring.IndexOf('<');
            if (nextTagIdx != -1) substring = substring.Substring(0, nextTagIdx);
            
            return substring.Trim();
        }

        private decimal? ExtractNumericWithLakh(string text, string prefix, string context)
        {
            int idx = text.IndexOf(context, StringComparison.OrdinalIgnoreCase);
            if (idx == -1) return null;
            
            var match = Regex.Match(text.Substring(idx), prefix + @"\s*([\d\.]+)\s*lakh", RegexOptions.IgnoreCase);
            if (match.Success && decimal.TryParse(match.Groups[1].Value, out decimal num))
            {
                return num * 100000;
            }
            return null;
        }
    }
}
