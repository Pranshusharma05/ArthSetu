using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArthSetuBackend.Services
{
    public class NdfdcConnectorService : IGovernmentConnector
    {
        private readonly HttpClient _httpClient;
        private readonly SourceSyncService _syncService;
        private readonly string _url = "https://ndfdc.nic.in/schemes"; 

        public NdfdcConnectorService(HttpClient httpClient, SourceSyncService syncService)
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
                return await _syncService.RunSyncTransactionAsync("src-ndfdc", "NdfdcConnectorService", _url, payload, 0, contentType, new List<SchemeStaging>());
            }
            
            var staging = ParseHtml(payload);
            
            return await _syncService.RunSyncTransactionAsync(
                "src-ndfdc",
                "NdfdcConnectorService",
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

            // Extract from html based on known schemes
            if (html.Contains("Divyangjan", StringComparison.OrdinalIgnoreCase) && html.Contains("Swavalamban", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(new SchemeStaging
                {
                    SchemeId = "ndfdc-dsy",
                    Name = "Divyangjan Swavalamban Yojana",
                    Description = ExtractTextNear(html, "Divyangjan Swavalamban Yojana", 150) ?? "For starting/expanding businesses, pursuing higher education (after class 12th)",
                    SchemeCategory = "Term Loan",
                    IsPwD = true,
                    DisabilityPercentageMin = 40,
                    LoanMax = ExtractNumericWithLakh(html, @"(?:upto|up to)\s+(?:Rs\.?|₹)", "Divyangjan Swavalamban Yojana") ?? 5000000,
                    LoanMaxRaw = "50 Lakhs",
                    SourceSection = "Divyangjan Swavalamban Yojana"
                });
                
                // Note: The education loan component is modeled as part of Divyangjan Swavalamban Yojana,
                // as per official documentation "For starting/expanding businesses, pursuing higher education..."
                // If a separate Education Loan scheme is found explicitly, it would be added below, but here we don't.
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
