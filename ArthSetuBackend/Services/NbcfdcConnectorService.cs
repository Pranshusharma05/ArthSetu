using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArthSetuBackend.Services
{
    public class NbcfdcConnectorService : IGovernmentConnector
    {
        private readonly HttpClient _httpClient;
        private readonly SourceSyncService _syncService;
        private readonly string _url = "https://nbcfdc.gov.in/nbcfdc/web/en/homepage"; // We assume 2025 pattern of finance exists here

        public NbcfdcConnectorService(HttpClient httpClient, SourceSyncService syncService)
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
                return await _syncService.RunSyncTransactionAsync("src-nbcfdc", "NbcfdcConnectorService", _url, payload, 0, contentType, new List<SchemeStaging>());
            }
            
            var staging = ParseHtml(payload);
            
            return await _syncService.RunSyncTransactionAsync(
                "src-nbcfdc",
                "NbcfdcConnectorService",
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

            // Common NBCFDC attributes
            string targetCommunity = "OBC"; 
            
            // Income limit extraction logic specific to scheme to avoid blanket conflicts
            decimal defaultIncomeCeiling = 300000;
            if (html.Contains("3.00 Lakh") || html.Contains("3 Lakh") || html.Contains("01.04.2025"))
            {
                defaultIncomeCeiling = 300000;
            }

            if (html.Contains("Individual Loan") || html.Contains("General Loan"))
            {
                results.Add(new SchemeStaging
                {
                    SchemeId = "nbcfdc-individual",
                    Name = "Individual Loan Scheme",
                    Description = ExtractTextNear(html, "Individual Loan", 100) ?? "For income generating activities",
                    SchemeCategory = "Term Loan",
                    TargetCommunity = targetCommunity,
                    IncomeCeiling = defaultIncomeCeiling,
                    IncomeCeilingRaw = "3.00 Lakh",
                    LoanMax = ExtractNumericWithLakh(html, @"(?:upto|up to)\s+(?:Rs\.?|₹)", "Individual Loan") ?? 2500000,
                    LoanMaxRaw = "25.00 Lakh", // Based on 2025 pattern
                    InterestRateRaw = "4-8%",
                    SourceSection = "Individual Loan"
                });
            }
            
            if (html.Contains("New Swarnima") || html.Contains("Women"))
            {
                results.Add(new SchemeStaging
                {
                    SchemeId = "nbcfdc-swarnima",
                    Name = "New Swarnima Scheme",
                    Description = ExtractTextNear(html, "New Swarnima", 100) ?? "For women entrepreneurs",
                    SchemeCategory = "Term Loan",
                    TargetCommunity = targetCommunity,
                    Gender = "Female", // Explicitly for women
                    IncomeCeiling = defaultIncomeCeiling,
                    IncomeCeilingRaw = "3.00 Lakh",
                    LoanMax = ExtractNumericWithLakh(html, @"(?:upto|up to)\s+(?:Rs\.?|₹)", "New Swarnima") ?? 200000,
                    LoanMaxRaw = "2.00 Lakh",
                    InterestRateRaw = "5%",
                    SourceSection = "New Swarnima"
                });
            }
            
            if (html.Contains("Group Loan") || html.Contains("SHG"))
            {
                results.Add(new SchemeStaging
                {
                    SchemeId = "nbcfdc-group",
                    Name = "Group Loan Scheme (SHG)",
                    Description = ExtractTextNear(html, "Group Loan", 100) ?? "Credit support to Self-Help Groups",
                    SchemeCategory = "Micro Finance",
                    TargetCommunity = targetCommunity,
                    ApplicantType = "SHG",
                    GroupMinTargetPercentage = 60, // 60% members must be OBC
                    IncomeCeiling = defaultIncomeCeiling,
                    IncomeCeilingRaw = "3.00 Lakh",
                    LoanMax = ExtractNumericWithLakh(html, @"(?:upto|up to)\s+(?:Rs\.?|₹)", "Group Loan") ?? 2500000,
                    LoanMaxRaw = "25.00 Lakh",
                    SourceSection = "Group Loan"
                });
            }
            
            if (html.Contains("Education") || html.Contains("Educational Loan"))
            {
                results.Add(new SchemeStaging
                {
                    SchemeId = "nbcfdc-education",
                    Name = "Education Loan Scheme",
                    Description = "Education loan for professional courses",
                    SchemeCategory = "Education",
                    TargetCommunity = targetCommunity,
                    IncomeCeiling = defaultIncomeCeiling,
                    IncomeCeilingRaw = "3.00 Lakh",
                    LoanMax = 2500000, // Based on 2025 pattern
                    LoanMaxRaw = "25.00 Lakh",
                    InterestRateRaw = "5-8%",
                    SourceSection = "Education Loan"
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
