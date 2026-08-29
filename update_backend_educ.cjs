const fs = require('fs');

const serverFile = 'server.ts';
let content = fs.readFileSync(serverFile, 'utf-8');

// Replace SCHEMES and everything up to the end of app.post('/api/schemes/match', ...)
// Let's find the start of "// --- Development Seed Data (Mocking SQL Server) ---"
const startMarker = '// --- Development Seed Data (Mocking SQL Server) ---';
// Let's find the next route or the end of match route.
// Wait, we can just replace everything from startMarker to the end of match block.
// But we might miss something.
// Let's just create a completely new server.ts that preserves the imports and other routes (districts).

const newBackendData = `
// --- Real Government Data Architectures (Mocking SQL Server Database) ---
const GOVERNMENT_SOURCES = [
  { id: 'src-nsp', name: 'National Scholarship Portal', ministry: 'Ministry of Electronics & Information Technology', domain: 'scholarships.gov.in', level: 'Central', ingestionMethod: 'Manual / Document-Based Verification Required', status: 'Verified' },
  { id: 'src-moe', name: 'Ministry of Education', department: 'Higher Education', domain: 'education.gov.in', level: 'Central', ingestionMethod: 'Manual / Document-Based Verification Required', status: 'Verified' },
  { id: 'src-msje', name: 'Ministry of Social Justice & Empowerment', domain: 'socialjustice.gov.in', level: 'Central', ingestionMethod: 'Official JSON', status: 'Verified' },
  { id: 'src-vidyalaxmi', name: 'PM-Vidyalaxmi', department: 'Department of Financial Services', domain: 'vidyalakshmi.co.in', level: 'Central', ingestionMethod: 'Official API', status: 'Pending Integration' },
  { id: 'src-up-swd', name: 'UP Scholarship and Fee Reimbursement Online System', department: 'Social Welfare Department', state: 'UP', domain: 'scholarship.up.gov.in', level: 'State', ingestionMethod: 'Manual / Document-Based Verification Required', status: 'Verified' }
];

const SCHEMES = [
  {
    id: 'edu-up-post-matric',
    name: 'Post Matric Scholarship for SC/ST/General/OBC/Minority (Uttar Pradesh)',
    sourceId: 'src-up-swd',
    category: 'Education',
    benefitType: 'Fee Reimbursement',
    purpose: 'Education',
    applicationRoute: 'Apply on State Scholarship Portal (scholarship.up.gov.in)',
    verificationStatus: 'Verified',
    lastVerified: '2023-11-01',
    description: 'Financial assistance and fee reimbursement for students studying at post-matriculation or post-secondary stage.',
    seedData: false
  },
  {
    id: 'edu-top-class-sc',
    name: 'Top Class Education Scheme for SC Students',
    sourceId: 'src-msje',
    category: 'Education',
    benefitType: 'Scholarship',
    purpose: 'Education',
    applicationRoute: 'Apply on National Scholarship Portal',
    verificationStatus: 'Verified',
    lastVerified: '2023-12-10',
    description: 'Financial support to SC students for pursuing degree and post graduate level courses in notified Top Class Institutions.',
    seedData: false
  },
  {
    id: 'edu-central-sector',
    name: 'Central Sector Scheme of Scholarship for College and University Students',
    sourceId: 'src-moe',
    category: 'Education',
    benefitType: 'Scholarship',
    purpose: 'Education',
    applicationRoute: 'Apply on National Scholarship Portal',
    verificationStatus: 'Verified',
    lastVerified: '2024-01-15',
    description: 'Financial assistance to meritorious students from low-income families to meet a part of their day-to-day expenses while pursuing higher studies.',
    seedData: false
  },
  {
    id: 'edu-interest-subsidy',
    name: 'Dr. Ambedkar Central Sector Scheme of Interest Subsidy on Educational Loans for Overseas Studies',
    sourceId: 'src-msje',
    category: 'Education',
    benefitType: 'Interest Subsidy',
    purpose: 'Education',
    applicationRoute: 'Apply through PM-Vidyalaxmi',
    verificationStatus: 'Verified',
    lastVerified: '2023-10-25',
    description: 'Interest subsidy on educational loans for overseas studies for OBCs and EBCs.',
    seedData: false
  }
];

const SCHEME_RULES = [
  // UP Post Matric
  { schemeId: 'edu-up-post-matric', field: 'state', operator: 'Equals', value: 'UP', mandatory: true },
  { schemeId: 'edu-up-post-matric', field: 'income', operator: 'Max_UP_PostMatric', value: '', mandatory: true }, // Custom logic required
  { schemeId: 'edu-up-post-matric', field: 'educationLevel', operator: 'InList', value: 'Undergraduate,Postgraduate,Vocational', mandatory: true },
  { schemeId: 'edu-up-post-matric', field: 'institutionRecognition', operator: 'Equals', value: 'Recognized', mandatory: true },

  // Top Class Education SC
  { schemeId: 'edu-top-class-sc', field: 'category', operator: 'Equals', value: 'SC', mandatory: true },
  { schemeId: 'edu-top-class-sc', field: 'income', operator: 'Max', value: '800000', mandatory: true },
  { schemeId: 'edu-top-class-sc', field: 'educationLevel', operator: 'InList', value: 'Undergraduate,Postgraduate', mandatory: true },
  { schemeId: 'edu-top-class-sc', field: 'institutionCategory', operator: 'Equals', value: 'Top Class Notified', mandatory: true },

  // Central Sector Scholarship
  { schemeId: 'edu-central-sector', field: 'income', operator: 'Max', value: '450000', mandatory: true },
  { schemeId: 'edu-central-sector', field: 'age', operator: 'Range', value: '18-25', mandatory: true },
  { schemeId: 'edu-central-sector', field: 'class12Marks', operator: 'Min', value: '80', mandatory: true },
  { schemeId: 'edu-central-sector', field: 'regularFullTime', operator: 'Equals', value: 'Yes', mandatory: true },
  { schemeId: 'edu-central-sector', field: 'educationLevel', operator: 'InList', value: 'Undergraduate,Postgraduate', mandatory: true },

  // Dr Ambedkar Interest Subsidy
  { schemeId: 'edu-interest-subsidy', field: 'category', operator: 'InList', value: 'OBC,EBC', mandatory: true }, // Not directly EBC mapped from category yet, we'll map category
  { schemeId: 'edu-interest-subsidy', field: 'income', operator: 'Max', value: '800000', mandatory: true },
  { schemeId: 'edu-interest-subsidy', field: 'educationLevel', operator: 'InList', value: 'Postgraduate', mandatory: true }, // For overseas studies, usually PG
];

const STATES_MAP: Record<string, string> = {
  'UP': 'Uttar Pradesh',
  'DL': 'Delhi',
  'MH': 'Maharashtra'
};

function resolveValue(profile: any, field: string) {
  if (profile[field] !== undefined && profile[field] !== null && profile[field] !== '') {
    return profile[field];
  }
  if (profile.dynamicAnswers && profile.dynamicAnswers[field] !== undefined && profile.dynamicAnswers[field] !== null && profile.dynamicAnswers[field] !== '') {
    return profile.dynamicAnswers[field];
  }
  return null;
}

function evaluateRules(schemeId: string, profile: any) {
  const rules = SCHEME_RULES.filter(r => r.schemeId === schemeId);
  const passed: any[] = [];
  const failed: any[] = [];
  const missing: any[] = [];

  for (const rule of rules) {
    // Determine base age
    let age = -1;
    if (profile.dob) {
      const dobDate = new Date(profile.dob);
      if (!isNaN(dobDate.getTime())) {
        const today = new Date();
        age = today.getFullYear() - dobDate.getFullYear();
        if (today.getMonth() < dobDate.getMonth() || (today.getMonth() === dobDate.getMonth() && today.getDate() < dobDate.getDate())) {
          age--;
        }
      }
    }

    let userValue = resolveValue(profile, rule.field);

    if (rule.field === 'age' && age !== -1) {
      userValue = age;
    }

    if (userValue === null) {
      missing.push(rule.field);
      continue;
    }

    let isPass = false;
    let expectedCondition = rule.value;
    let providedCondition = String(userValue);

    if (rule.operator === 'Equals') {
      isPass = String(userValue).toLowerCase() === String(rule.value).toLowerCase();
    } else if (rule.operator === 'InList') {
      const list = rule.value.split(',').map(s => s.trim().toLowerCase());
      isPass = list.includes(String(userValue).toLowerCase());
    } else if (rule.operator === 'Max') {
      isPass = Number(userValue) <= Number(rule.value);
      expectedCondition = \`Max \${rule.value}\`;
    } else if (rule.operator === 'Min') {
      isPass = Number(userValue) >= Number(rule.value);
      expectedCondition = \`Min \${rule.value}\`;
    } else if (rule.operator === 'Range') {
      const [min, max] = rule.value.split('-').map(Number);
      isPass = Number(userValue) >= min && Number(userValue) <= max;
      expectedCondition = \`Between \${min} and \${max}\`;
    } else if (rule.operator === 'Max_UP_PostMatric') {
      const isSCST = (profile.category === 'SC' || profile.category === 'ST');
      const limit = isSCST ? 250000 : 200000;
      isPass = Number(userValue) <= limit;
      expectedCondition = \`Max \${limit} for \${profile.category}\`;
    }

    if (isPass) {
      passed.push({ ruleName: rule.field, userValue: providedCondition, schemeCondition: expectedCondition, status: 'Matched' });
    } else {
      failed.push({ ruleName: rule.field, userValue: providedCondition, schemeCondition: expectedCondition, status: 'Failed' });
    }
  }

  let eligibilityState = 'Eligible';
  if (failed.length > 0) {
    eligibilityState = 'Not Eligible';
  } else if (missing.length > 0) {
    eligibilityState = 'More Information Needed';
  }

  return { eligibilityState, passed, failed, missing };
}
`;

// Insert the new logic before app.listen or at a specific point
// Let's rewrite server.ts carefully.
const newServerContent = `
import express from "express";
import path from "path";
import { createServer as createViteServer } from "vite";

async function startServer() {
  const app = express();
  const PORT = 3000;
  
  app.use(express.json());

${newBackendData}

  app.get("/api/locations/districts", (req, res) => {
    const state = req.query.state as string;
    if (state === 'UP') {
      res.json([
        { code: 'AL', name: 'Aligarh' }, { code: 'AG', name: 'Agra' },
        { code: 'LU', name: 'Lucknow' }, { code: 'KA', name: 'Kanpur Nagar' },
        { code: 'VA', name: 'Varanasi' }, { code: 'ME', name: 'Meerut' }
      ]);
    } else if (state === 'MH') {
      res.json([
        { code: 'MU', name: 'Mumbai City' }, { code: 'MS', name: 'Mumbai Suburban' },
        { code: 'PU', name: 'Pune' }, { code: 'NA', name: 'Nagpur' },
        { code: 'TH', name: 'Thane' }, { code: 'NS', name: 'Nashik' }
      ]);
    } else if (state === 'DL') {
      res.json([
        { code: 'ND', name: 'New Delhi' }, { code: 'CD', name: 'Central Delhi' },
        { code: 'SD', name: 'South Delhi' }, { code: 'ED', name: 'East Delhi' },
        { code: 'WD', name: 'West Delhi' }
      ]);
    } else {
      const stateName = STATES_MAP[state] || 'State';
      res.json([
        { code: \`\${state}-01\`, name: \`\${stateName} District 1\` },
        { code: \`\${state}-02\`, name: \`\${stateName} District 2\` }
      ]);
    }
  });

  app.post("/api/schemes/dynamic-questions", (req, res) => {
    const profile = req.body;
    if (profile.purpose !== 'Education') {
       // fallback mock for Business
       return res.json([
         { id: 'businessActivity', type: 'taxonomy', label: 'Select Business / Economic Activity' },
         { id: 'projectCost', type: 'currency', label: 'Estimated Project Cost (₹)', helpText: 'Total amount needed for your business or project' }
       ]);
    }

    const allMissing = new Set<string>();
    
    for (const scheme of SCHEMES.filter(s => s.purpose === 'Education')) {
      const evalRes = evaluateRules(scheme.id, profile);
      if (evalRes.eligibilityState !== 'Not Eligible') {
        evalRes.missing.forEach(m => allMissing.add(m));
      }
    }

    const questions: any[] = [];
    if (allMissing.has('educationLevel')) {
      questions.push({
        id: 'educationLevel', type: 'single_choice', label: 'Education Level',
        options: [{label: 'Undergraduate', value: 'Undergraduate'}, {label: 'Postgraduate', value: 'Postgraduate'}, {label: 'Vocational', value: 'Vocational'}]
      });
    }
    if (allMissing.has('course')) {
      questions.push({ id: 'course', type: 'text', label: 'Course Name (e.g. B.Tech)' });
    }
    if (allMissing.has('institutionRecognition')) {
      questions.push({
        id: 'institutionRecognition', type: 'single_choice', label: 'Is your Institution officially Recognized?',
        options: [{label: 'Yes, Recognized', value: 'Recognized'}, {label: 'No / Unrecognized', value: 'Unrecognized'}]
      });
    }
    if (allMissing.has('institutionCategory')) {
      questions.push({
        id: 'institutionCategory', type: 'single_choice', label: 'Institution Category',
        options: [{label: 'Top Class Notified', value: 'Top Class Notified'}, {label: 'Other', value: 'Other'}]
      });
    }
    if (allMissing.has('class12Marks')) {
      questions.push({ id: 'class12Marks', type: 'numeric', label: 'Class XII Marks / Percentile' });
    }
    if (allMissing.has('regularFullTime')) {
      questions.push({
        id: 'regularFullTime', type: 'yes_no', label: 'Is this a Regular / Full-Time course?',
        options: [{label: 'Yes', value: 'Yes'}, {label: 'No', value: 'No'}]
      });
    }
    if (allMissing.has('annualCourseFee')) {
      questions.push({ id: 'annualCourseFee', type: 'currency', label: 'Annual Course Fee (₹)' });
    }
    if (allMissing.has('totalCourseCost')) {
      questions.push({ id: 'totalCourseCost', type: 'currency', label: 'Total Course Cost (₹)' });
    }

    res.json(questions);
  });

  app.post("/api/schemes/match", (req, res) => {
    const profile = req.body;
    const recommended: any[] = [];
    const otherEligible: any[] = [];
    const moreInfoNeeded: any[] = [];
    const notEligible: any[] = [];

    // Map through schemes
    const activeSchemes = SCHEMES.filter(s => s.purpose === profile.purpose || profile.purpose === 'Education'); 
    
    // Default fallback if not Education for demo stability
    if (profile.purpose !== 'Education') {
        res.json({ recommended: [], otherEligible: [], moreInfoNeeded: [], notEligible: [] });
        return;
    }

    for (const scheme of activeSchemes) {
       const evalRes = evaluateRules(scheme.id, profile);
       const source = GOVERNMENT_SOURCES.find(s => s.id === scheme.sourceId);
       
       const schemeData = {
         ...scheme,
         officialSource: source?.name || 'Government Source',
         ruleComparisons: [...evalRes.passed, ...evalRes.failed],
         missingRules: evalRes.missing
       };

       if (evalRes.eligibilityState === 'Not Eligible') {
         notEligible.push(schemeData);
       } else if (evalRes.eligibilityState === 'More Information Needed') {
         moreInfoNeeded.push(schemeData);
       } else {
         recommended.push(schemeData);
       }
    }

    res.json({
      recommended,
      otherEligible,
      moreInfoNeeded,
      notEligible
    });
  });

  if (process.env.NODE_ENV !== "production") {
    const vite = await createViteServer({
      server: { middlewareMode: true },
      appType: "spa",
    });
    app.use(vite.middlewares);
  } else {
    const distPath = path.join(process.cwd(), 'dist');
    app.use(express.static(distPath));
    app.get('*', (req, res) => {
      res.sendFile(path.join(distPath, 'index.html'));
    });
  }

  app.listen(PORT, "0.0.0.0", () => {
    console.log(\`Server running on http://localhost:\${PORT}\`);
  });
}

startServer();
`;

fs.writeFileSync('server.ts', newServerContent, 'utf-8');
