// NON-PRODUCTION LEGACY MOCK SERVER - NOT USED BY CITIZEN PRODUCTION FLOW (Use ASP.NET Backend on Port 5000)
import express from "express";
import path from "path";
import { createServer as createViteServer } from "vite";

async function startServer() {
  const app = express();
  const PORT = 3000;
  
  app.use(express.json());

const GOVERNMENT_SOURCES = [
  { id: 'src-nsp', name: 'National Scholarship Portal', ministry: 'Ministry of Electronics & Information Technology', domain: 'scholarships.gov.in', level: 'Central', ingestionMethod: 'API', status: 'Verified' },
  { id: 'src-moe', name: 'Ministry of Education', department: 'Higher Education', domain: 'education.gov.in', level: 'Central', ingestionMethod: 'API', status: 'Verified' },
  { id: 'src-msje', name: 'Ministry of Social Justice & Empowerment', domain: 'socialjustice.gov.in', level: 'Central', ingestionMethod: 'API', status: 'Verified' },
  { id: 'src-msme', name: 'Ministry of MSME', domain: 'msme.gov.in', level: 'Central', ingestionMethod: 'API', status: 'Verified' },
  { id: 'src-dfs', name: 'Department of Financial Services', domain: 'financialservices.gov.in', level: 'Central', ingestionMethod: 'API', status: 'Verified' },
  { id: 'src-dpiit', name: 'DPIIT', domain: 'startupindia.gov.in', level: 'Central', ingestionMethod: 'API', status: 'Verified' },
  { id: 'src-moa', name: 'Ministry of Agriculture', domain: 'agricoop.gov.in', level: 'Central', ingestionMethod: 'API', status: 'Verified' },
  { id: 'src-mofpi', name: 'Ministry of Food Processing Industries', domain: 'mofpi.gov.in', level: 'Central', ingestionMethod: 'API', status: 'Verified' },
  { id: 'src-mohua', name: 'Ministry of Housing and Urban Affairs', domain: 'mohua.gov.in', level: 'Central', ingestionMethod: 'API', status: 'Verified' },
  { id: 'src-up-swd', name: 'UP Social Welfare Department', domain: 'scholarship.up.gov.in', level: 'State', ingestionMethod: 'API', status: 'Verified' },
  { id: 'src-msma', name: 'Ministry of Minority Affairs', domain: 'minorityaffairs.gov.in', level: 'Central', ingestionMethod: 'API', status: 'Verified' },
];

const SCHEMES = [
  // EDUCATION
  { id: 'edu-up-post-matric', purpose: 'Education', name: 'Post Matric Scholarship (UP)', sourceId: 'src-up-swd', category: 'Scholarship', lifecycle: 'ACTIVE_AND_OPEN' },
  { id: 'edu-top-class-sc', purpose: 'Education', name: 'Top Class Education Scheme for SC', sourceId: 'src-msje', category: 'Scholarship', lifecycle: 'ACTIVE_AND_OPEN' },
  { id: 'edu-pm-usp-central-sector', purpose: 'Education', name: 'PM-USP Central Sector Scheme of Scholarship for College and University Students', sourceId: 'src-moe', category: 'Scholarship', lifecycle: 'ACTIVE_AND_OPEN' },
  { id: 'edu-pragati-degree', purpose: 'Education', name: 'AICTE Pragati Scholarship for Girls - Degree', sourceId: 'src-moe', category: 'Scholarship', lifecycle: 'ACTIVE_AND_OPEN' },
  { id: 'edu-pragati-diploma', purpose: 'Education', name: 'AICTE Pragati Scholarship for Girls - Diploma', sourceId: 'src-moe', category: 'Scholarship', lifecycle: 'ACTIVE_AND_OPEN' },
  { id: 'edu-saksham-degree', purpose: 'Education', name: 'AICTE Saksham Scholarship for Specially-Abled Student - Degree', sourceId: 'src-moe', category: 'Scholarship', lifecycle: 'ACTIVE_AND_OPEN' },
  { id: 'edu-saksham-diploma', purpose: 'Education', name: 'AICTE Saksham Scholarship for Specially-Abled Student - Diploma', sourceId: 'src-moe', category: 'Scholarship', lifecycle: 'ACTIVE_AND_OPEN' },
  
  // EDUCATION LOAN
  { id: 'edu-interest-subsidy', purpose: 'Education', name: 'Dr. Ambedkar Central Sector Scheme of Interest Subsidy on Educational Loans for Overseas Studies for OBCs/EBCs', sourceId: 'src-msje', category: 'Education Loan', lifecycle: 'ACTIVE_AND_OPEN' },
  { id: 'edu-padho-pardesh', purpose: 'Education', name: 'Padho Pardesh Scheme of Interest Subsidy', sourceId: 'src-msma', category: 'Education Loan', lifecycle: 'DISCONTINUED' },
  
  // BUSINESS
  { id: 'biz-pmegp', purpose: 'Business', name: 'Prime Minister Employment Generation Programme (PMEGP)', sourceId: 'src-msme', category: 'Loan', lifecycle: 'ACTIVE_AND_OPEN' },
  { id: 'biz-pmfme', purpose: 'Business', name: 'PM Formalisation of Micro food processing Enterprises (PMFME)', sourceId: 'src-mofpi', category: 'Loan', lifecycle: 'ACTIVE_AND_OPEN', components: ['Individual Micro Enterprises'] },
  { id: 'biz-pmmy', purpose: 'Business', name: 'Pradhan Mantri MUDRA Yojana (PMMY)', sourceId: 'src-dfs', category: 'Loan', lifecycle: 'ACTIVE_AND_OPEN', components: ['Shishu', 'Kishore', 'Tarun', 'Tarun Plus'] },
  
  // STARTUP
  { id: 'startup-sisfs', purpose: 'Startup', name: 'Startup India Seed Fund Scheme (SISFS)', sourceId: 'src-dpiit', category: 'Grant', lifecycle: 'ACTIVE_BUT_APPLICATION_CLOSED', applicationWindowStatus: 'CLOSED' },
  { id: 'startup-cgss', purpose: 'Startup', name: 'Credit Guarantee Scheme for Startups (CGSS)', sourceId: 'src-dpiit', category: 'Guarantee', lifecycle: 'ACTIVE_AND_OPEN' },
  
  // AGRICULTURE
  { id: 'agri-pmkisan', purpose: 'Agriculture', name: 'PM-KISAN', sourceId: 'src-moa', category: 'Direct Benefit', lifecycle: 'ACTIVE_AND_OPEN' },
  { id: 'agri-aif', purpose: 'Agriculture', name: 'Agriculture Infrastructure Fund (AIF)', sourceId: 'src-moa', category: 'Loan', lifecycle: 'ACTIVE_AND_OPEN' },
  { id: 'agri-kcc', purpose: 'Agriculture', name: 'Kisan Credit Card (KCC)', sourceId: 'src-moa', category: 'Credit', lifecycle: 'ACTIVE_AND_OPEN' }
];

const SCHEME_RULES = [
  // Edu Scholarship
  { schemeId: 'edu-up-post-matric', field: 'state', operator: 'Equals', value: 'UP', provenance: 'SYSTEM_DERIVED' },
  { schemeId: 'edu-top-class-sc', field: 'category', operator: 'Equals', value: 'SC', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-pm-usp-central-sector', field: 'income', operator: 'Max', value: '450000', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-pragati-degree', field: 'gender', operator: 'Equals', value: 'Female', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-pragati-degree', field: 'courseType', operator: 'Equals', value: 'Degree', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-pragati-diploma', field: 'gender', operator: 'Equals', value: 'Female', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-pragati-diploma', field: 'courseType', operator: 'Equals', value: 'Diploma', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-saksham-degree', field: 'pwdStatus', operator: 'Equals', value: 'Yes', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-saksham-degree', field: 'disabilityPercentage', operator: 'Min', value: '40', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-saksham-degree', field: 'courseType', operator: 'Equals', value: 'Degree', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-saksham-diploma', field: 'pwdStatus', operator: 'Equals', value: 'Yes', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-saksham-diploma', field: 'disabilityPercentage', operator: 'Min', value: '40', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-saksham-diploma', field: 'courseType', operator: 'Equals', value: 'Diploma', provenance: 'USER_DECLARED' },
  
  // Edu Loan
  { schemeId: 'edu-interest-subsidy', field: 'category', operator: 'InList', value: 'OBC,EBC', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-interest-subsidy', field: 'studyLocation', operator: 'Equals', value: 'Overseas', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-padho-pardesh', field: 'category', operator: 'Equals', value: 'Minority', provenance: 'USER_DECLARED' },
  { schemeId: 'edu-padho-pardesh', field: 'studyLocation', operator: 'Equals', value: 'Overseas', provenance: 'USER_DECLARED' },

  // Business (PMEGP)
  { schemeId: 'biz-pmegp', field: 'age', operator: 'Min', value: '18', provenance: 'SYSTEM_DERIVED' },
  { schemeId: 'biz-pmegp', field: 'projectCost', operator: 'Max', value: '5000000', provenance: 'USER_DECLARED' },
  { schemeId: 'biz-pmegp', field: 'businessActivity', operator: 'InList', value: 'Manufacturing,Service', provenance: 'USER_DECLARED' },
  { schemeId: 'biz-pmegp', field: 'applicantType', operator: 'InList', value: 'Individual,SHG,Institution', provenance: 'USER_DECLARED' },
  
  // Business (PMFME)
  { schemeId: 'biz-pmfme', field: 'businessActivity', operator: 'Equals', value: 'Food Processing', provenance: 'USER_DECLARED' },
  { schemeId: 'biz-pmfme', field: 'enterpriseType', operator: 'InList', value: 'Micro,FPO,SHG', provenance: 'USER_DECLARED' },

  // Business (PMMY Canonical)
  { schemeId: 'biz-pmmy', field: 'projectCost', operator: 'Max', value: '2000000', provenance: 'USER_DECLARED' },

  // Startup (SISFS - Comprehensive)
  { schemeId: 'startup-sisfs', field: 'dpiitRecognized', operator: 'Equals', value: 'Yes', provenance: 'OFFICIAL_DATABASE_VERIFIED' },
  { schemeId: 'startup-sisfs', field: 'incorporationYears', operator: 'Max', value: '2', provenance: 'SYSTEM_DERIVED' },
  { schemeId: 'startup-sisfs', field: 'priorGovtSupportLimit', operator: 'Equals', value: 'Yes', provenance: 'USER_DECLARED' },
  { schemeId: 'startup-sisfs', field: 'businessIdeaFit', operator: 'Equals', value: 'Yes', provenance: 'USER_DECLARED' },
  { schemeId: 'startup-sisfs', field: 'technologyUsage', operator: 'Equals', value: 'Yes', provenance: 'USER_DECLARED' },
  { schemeId: 'startup-sisfs', field: 'indianPromoterShareholding', operator: 'Equals', value: 'Yes', provenance: 'USER_DECLARED' },
  { schemeId: 'startup-sisfs', field: 'previousSeedSupportRestrictions', operator: 'Equals', value: 'Yes', provenance: 'USER_DECLARED' },

  // Startup (CGSS - Comprehensive)
  { schemeId: 'startup-cgss', field: 'dpiitRecognized', operator: 'Equals', value: 'Yes', provenance: 'OFFICIAL_DATABASE_VERIFIED' },
  { schemeId: 'startup-cgss', field: 'notInDefault', operator: 'Equals', value: 'Yes', provenance: 'EXTERNAL_AUTHORITY_ASSESSMENT' },
  { schemeId: 'startup-cgss', field: 'notNpa', operator: 'Equals', value: 'Yes', provenance: 'EXTERNAL_AUTHORITY_ASSESSMENT' },
  { schemeId: 'startup-cgss', field: 'memberInstitutionEligibility', operator: 'Equals', value: 'Yes', provenance: 'EXTERNAL_AUTHORITY_ASSESSMENT' },
  
  // Agriculture (PM-KISAN Exclusions)
  { schemeId: 'agri-pmkisan', field: 'landholding', operator: 'Max', value: '2', provenance: 'USER_DECLARED' },
  { schemeId: 'agri-pmkisan', field: 'institutionalLandHolder', operator: 'Equals', value: 'No', provenance: 'USER_DECLARED' },
  { schemeId: 'agri-pmkisan', field: 'constitutionalPostHolder', operator: 'Equals', value: 'No', provenance: 'USER_DECLARED' },
  { schemeId: 'agri-pmkisan', field: 'govtEmployee', operator: 'Equals', value: 'No', provenance: 'USER_DECLARED' },
  { schemeId: 'agri-pmkisan', field: 'pensioner', operator: 'Equals', value: 'No', provenance: 'USER_DECLARED' },
  { schemeId: 'agri-pmkisan', field: 'incomeTaxPayer', operator: 'Equals', value: 'No', provenance: 'USER_DECLARED' },
  { schemeId: 'agri-pmkisan', field: 'registeredProfessional', operator: 'Equals', value: 'No', provenance: 'USER_DECLARED' },

  { schemeId: 'agri-aif', field: 'agriActivity', operator: 'Equals', value: 'PostHarvest', provenance: 'USER_DECLARED' },
  { schemeId: 'agri-kcc', field: 'farmerType', operator: 'InList', value: 'Owner,Tenant,Sharecropper', provenance: 'USER_DECLARED' }
];

const QUESTIONS_DEF = [
  { id: 'state', type: 'text', label: 'State' },
  { id: 'category', type: 'single_choice', label: 'Social Category', options: [{label:'SC',value:'SC'},{label:'ST',value:'ST'},{label:'OBC',value:'OBC'},{label:'General',value:'General'},{label:'Minority',value:'Minority'},{label:'EBC',value:'EBC'}] },
  { id: 'gender', type: 'single_choice', label: 'Gender', options: [{label:'Male',value:'Male'},{label:'Female',value:'Female'},{label:'Other',value:'Other'}] },
  { id: 'pwdStatus', type: 'yes_no', label: 'Are you a Person with Disability (PwD)?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  { id: 'disabilityPercentage', type: 'numeric', label: 'Disability Percentage' },
  { id: 'courseType', type: 'single_choice', label: 'Course Type', options: [{label:'Degree',value:'Degree'},{label:'Diploma',value:'Diploma'},{label:'Other',value:'Other'}] },
  { id: 'studyLocation', type: 'single_choice', label: 'Study Location', options: [{label:'India',value:'India'},{label:'Overseas',value:'Overseas'}] },
  { id: 'income', type: 'currency', label: 'Annual Family Income (₹)' },
  { id: 'age', type: 'numeric', label: 'Age' },
  
  { id: 'projectCost', type: 'currency', label: 'Estimated Project Cost (₹)' },
  { id: 'businessActivity', type: 'single_choice', label: 'Business Sector', options: [{label:'Manufacturing',value:'Manufacturing'},{label:'Service',value:'Service'},{label:'Trading',value:'Trading'},{label:'Food Processing',value:'Food Processing'}] },
  { id: 'applicantType', type: 'single_choice', label: 'Applicant Type', options: [{label:'Individual',value:'Individual'},{label:'SHG',value:'SHG'},{label:'Institution',value:'Institution'}] },
  { id: 'educationLevel', type: 'single_choice', label: 'Education Qualification', options: [{label:'Below 8th',value:'Below 8th'},{label:'8th Pass',value:'8th Pass'},{label:'10th Pass',value:'10th Pass'},{label:'Graduate',value:'Graduate'}] },
  { id: 'repaymentHistory', type: 'single_choice', label: 'Past Loan Repayment History', options: [{label:'Good',value:'Good'},{label:'Defaulted',value:'Defaulted'},{label:'No Past Loan',value:'No Past Loan'}] },
  { id: 'previousTarunLoan', type: 'yes_no', label: 'Have you fully repaid a previous PMMY Tarun loan?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  
  { id: 'dpiitRecognized', type: 'yes_no', label: 'Is your startup DPIIT Recognized?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  { id: 'incorporationYears', type: 'numeric', label: 'Years since incorporation' },
  { id: 'priorGovtSupportLimit', type: 'yes_no', label: 'Has prior Govt support exceeded permissible limit (₹10 Lakh)?', options: [{label:'Yes',value:'No'},{label:'No',value:'Yes'}] },
  { id: 'businessIdeaFit', type: 'yes_no', label: 'Do you have a viable business idea for scale?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  { id: 'technologyUsage', type: 'yes_no', label: 'Does your startup rely on technology in its core product/service?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  { id: 'indianPromoterShareholding', type: 'yes_no', label: 'Is Indian promoter shareholding at least 51%?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  { id: 'previousSeedSupportRestrictions', type: 'yes_no', label: 'Have you received seed fund before?', options: [{label:'Yes',value:'No'},{label:'No',value:'Yes'}] },

  { id: 'notInDefault', type: 'yes_no', label: 'Are you in default with any lender?', options: [{label:'Yes',value:'No'},{label:'No',value:'Yes'}] },
  { id: 'notNpa', type: 'yes_no', label: 'Is your account classified as NPA?', options: [{label:'Yes',value:'No'},{label:'No',value:'Yes'}] },
  { id: 'memberInstitutionEligibility', type: 'yes_no', label: 'Has the lender (Member Institution) approved eligibility?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'},{label:'Requires Assessment',value:'Requires Assessment'}] },
  
  { id: 'landholding', type: 'numeric', label: 'Landholding (in Hectares)' },
  { id: 'farmerType', type: 'single_choice', label: 'Farmer Type', options: [{label:'Small/Marginal',value:'Small/Marginal'},{label:'Owner',value:'Owner'},{label:'Tenant',value:'Tenant'},{label:'Sharecropper',value:'Sharecropper'}] },
  { id: 'agriActivity', type: 'single_choice', label: 'Agriculture Activity', options: [{label:'Cultivation',value:'Cultivation'},{label:'PostHarvest',value:'PostHarvest'}] },
  { id: 'enterpriseType', type: 'single_choice', label: 'Enterprise Type', options: [{label:'Micro',value:'Micro'},{label:'FPO',value:'FPO'},{label:'SHG',value:'SHG'},{label:'Small',value:'Small'}] },
  
  { id: 'institutionalLandHolder', type: 'yes_no', label: 'Are you an institutional land holder?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  { id: 'constitutionalPostHolder', type: 'yes_no', label: 'Do you hold a constitutional post?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  { id: 'govtEmployee', type: 'yes_no', label: 'Are you a current or former Government employee?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  { id: 'pensioner', type: 'yes_no', label: 'Is your pension >= ₹10,000?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  { id: 'incomeTaxPayer', type: 'yes_no', label: 'Did you pay Income Tax in the last assessment year?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] },
  { id: 'registeredProfessional', type: 'yes_no', label: 'Are you a registered Doctor, Engineer, Lawyer, etc.?', options: [{label:'Yes',value:'Yes'},{label:'No',value:'No'}] }
];

function resolveValue(profile, field) {
  if (profile[field] !== undefined && profile[field] !== null && profile[field] !== '') return profile[field];
  if (profile.dynamicAnswers && profile.dynamicAnswers[field] !== undefined && profile.dynamicAnswers[field] !== null && profile.dynamicAnswers[field] !== '') return profile.dynamicAnswers[field];
  return null;
}

function evaluateRules(schemeId, profile) {
  const rules = SCHEME_RULES.filter(r => r.schemeId === schemeId);
  
  // CONDITIONAL RULES INJECTION
  if (schemeId === 'biz-pmegp') {
    const pc = resolveValue(profile, 'projectCost');
    const act = resolveValue(profile, 'businessActivity');
    if (pc && act) {
       if ((act === 'Manufacturing' && Number(pc) > 1000000) || (act === 'Service' && Number(pc) > 500000)) {
           rules.push({ schemeId: 'biz-pmegp', field: 'educationLevel', operator: 'InList', value: '8th Pass,10th Pass,Graduate', provenance: 'USER_DECLARED' });
       }
    } else if (pc) {
       rules.push({ schemeId: 'biz-pmegp', field: 'educationLevel', operator: 'InList', value: '8th Pass,10th Pass,Graduate', provenance: 'USER_DECLARED' }); // Optimistic require
    }
  }

  // Tarun Plus conditional rule on PMMY
  if (schemeId === 'biz-pmmy') {
    const pc = resolveValue(profile, 'projectCost');
    if (pc && Number(pc) > 1000000) {
        rules.push({ schemeId: 'biz-pmmy', field: 'repaymentHistory', operator: 'Equals', value: 'Good', provenance: 'USER_DECLARED' });
        rules.push({ schemeId: 'biz-pmmy', field: 'previousTarunLoan', operator: 'Equals', value: 'Yes', provenance: 'USER_DECLARED' });
    }
  }

  const passed = [];
  const failed = [];
  const missing = [];
  let requiresAssessment = false;

  for (const rule of rules) {
    let userValue = resolveValue(profile, rule.field);
    if (userValue === null) {
      missing.push(rule.field);
      continue;
    }
    
    if (rule.provenance === 'EXTERNAL_AUTHORITY_ASSESSMENT' && userValue === 'Requires Assessment') {
       requiresAssessment = true;
       continue;
    }

    let isPass = false;
    if (rule.operator === 'Equals') isPass = String(userValue).toLowerCase() === String(rule.value).toLowerCase();
    else if (rule.operator === 'InList') isPass = rule.value.toLowerCase().split(',').includes(String(userValue).toLowerCase());
    else if (rule.operator === 'Max') isPass = Number(userValue) <= Number(rule.value);
    else if (rule.operator === 'Min') isPass = Number(userValue) >= Number(rule.value);
    else if (rule.operator === 'Range') {
      const [min, max] = rule.value.split('-').map(Number);
      isPass = Number(userValue) >= min && Number(userValue) <= max;
    }

    if (isPass) passed.push({ ruleName: rule.field, userValue: String(userValue), status: 'Matched', provenance: rule.provenance });
    else failed.push({ ruleName: rule.field, userValue: String(userValue), status: 'Failed', provenance: rule.provenance });
  }

  let eligibilityState = 'Eligible';
  if (failed.length > 0) eligibilityState = 'Not Eligible';
  else if (missing.length > 0) eligibilityState = 'More Information Needed';
  else if (requiresAssessment) eligibilityState = 'Requires Lender Assessment';

  return { eligibilityState, passed, failed, missing };
}

app.post("/api/schemes/dynamic-questions", (req, res) => {
  const profile = req.body;
  
  // Filter out discontinued schemes from active flow
  const activeSchemes = SCHEMES.filter(s => s.lifecycle !== 'DISCONTINUED' && s.lifecycle !== 'SUPERSEDED' && s.lifecycle !== 'HISTORICAL_ONLY' && (s.purpose === profile.purpose || (profile.purpose === 'Business' && s.purpose === 'Startup') || (profile.purpose === 'Startup' && s.purpose === 'Business')));
  
  if (activeSchemes.length === 0) return res.json([]);

  const missingCounts = new Map();
  
  for (const scheme of activeSchemes) {
    const evalRes = evaluateRules(scheme.id, profile);
    if (evalRes.eligibilityState !== 'Not Eligible') {
      for (const m of evalRes.missing) {
         missingCounts.set(m, (missingCounts.get(m) || 0) + 1);
      }
    }
  }

  if (missingCounts.size === 0) return res.json([]);

  let bestField = '';
  let maxCount = -1;
  for (const [field, count] of missingCounts.entries()) {
    if (count > maxCount) {
      maxCount = count;
      bestField = field;
    }
  }

  const qDef = QUESTIONS_DEF.find(q => q.id === bestField);
  if (qDef) {
    res.json([qDef]);
  } else {
    res.json([{ id: bestField, type: 'text', label: bestField }]);
  }
});

app.post("/api/schemes/match", (req, res) => {
  const profile = req.body;
  const recommended = [];
  const otherEligible = [];
  const moreInfoNeeded = [];
  const notEligible = [];

  const activeSchemes = SCHEMES.filter(s => s.lifecycle !== 'DISCONTINUED' && s.lifecycle !== 'SUPERSEDED' && s.lifecycle !== 'HISTORICAL_ONLY' && (s.purpose === profile.purpose || (profile.purpose === 'Business' && s.purpose === 'Startup') || (profile.purpose === 'Startup' && s.purpose === 'Business')));

  for (const scheme of activeSchemes) {
     const evalRes = evaluateRules(scheme.id, profile);
     const source = GOVERNMENT_SOURCES.find(s => s.id === scheme.sourceId);
     
     const schemeData = {
       ...scheme,
       officialSource: source?.name || 'Government Source',
       ruleComparisons: [...evalRes.passed, ...evalRes.failed, ...evalRes.missing.map(m => ({ ruleName: m, status: 'Missing' }))],
       missingRules: evalRes.missing
     };

     if (evalRes.eligibilityState === 'Not Eligible') notEligible.push(schemeData);
     else if (evalRes.eligibilityState === 'More Information Needed') moreInfoNeeded.push(schemeData);
     else recommended.push(schemeData);
  }

  res.json({ recommended, otherEligible, moreInfoNeeded, notEligible });
});

  if (process.env.NODE_ENV !== "production") {
    const vite = await createViteServer({ server: { middlewareMode: true }, appType: "spa" });
    app.use(vite.middlewares);
  } else {
    const distPath = path.join(process.cwd(), 'dist');
    app.use(express.static(distPath));
    app.get('*', (req, res) => res.sendFile(path.join(distPath, 'index.html')));
  }

  app.listen(PORT, "0.0.0.0", () => {
    console.log("Server running on http://localhost:" + PORT);
  });
}

startServer();
