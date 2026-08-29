const fs = require('fs');
let content = fs.readFileSync('server.ts', 'utf-8');

const newSchemes = `
    {
      id: "PM_KISAN",
      name: "Pradhan Mantri Kisan Samman Nidhi (PM-KISAN)",
      ministry: "Ministry of Agriculture and Farmers Welfare",
      scope: "Central",
      purpose: "Agriculture",
      benefitType: "Income Support",
      ageRules: null,
      incomeRules: null,
      locationRules: { scope: "Central" },
      activityConditions: ["Agriculture", "Dairy", "Poultry"],
      source: "pmkisan.gov.in",
      lastVerified: "2024-03-01",
      description: "Direct income support of ₹6,000 per year to all landholding farmer families.",
      applicationMode: "Online"
    },
    {
      id: "PMAY_G",
      name: "Pradhan Mantri Awas Yojana - Gramin",
      ministry: "Ministry of Rural Development",
      scope: "Central",
      purpose: "Housing",
      benefitType: "Financial Assistance",
      ageRules: null,
      incomeRules: null,
      locationRules: { scope: "Central" },
      source: "pmayg.nic.in",
      lastVerified: "2023-11-20",
      description: "Provides a pucca house with basic amenities to all houseless householder.",
      applicationMode: "Partner"
    },
    {
      id: "PMJAY",
      name: "Ayushman Bharat - PM-JAY",
      ministry: "Ministry of Health and Family Welfare",
      scope: "Central",
      purpose: "Health",
      benefitType: "Health Insurance",
      ageRules: null,
      incomeRules: null, // SECC database based, no hard income rule here
      locationRules: { scope: "Central" },
      source: "pmjay.gov.in",
      lastVerified: "2024-02-15",
      description: "Provides a health cover of ₹5 lakhs per family per year for secondary and tertiary care hospitalization.",
      applicationMode: "Partner"
    },
    {
      id: "PMKVY",
      name: "Pradhan Mantri Kaushal Vikas Yojana",
      ministry: "Ministry of Skill Development",
      scope: "Central",
      purpose: "Skills",
      benefitType: "Training",
      ageRules: { min: 15, max: 45 },
      incomeRules: null,
      locationRules: { scope: "Central" },
      source: "pmkvyofficial.org",
      lastVerified: "2023-10-10",
      description: "Aims to encourage youth to take up industry-relevant skill training.",
      applicationMode: "Partner"
    }
  ];`;

content = content.replace(/applicationMode: "Online"\n\s*\}\n\s*\];/, 'applicationMode: "Online"\n    },' + newSchemes);

fs.writeFileSync('server.ts', content, 'utf-8');
