const fs = require('fs');

// Patch server.ts
let serverContent = fs.readFileSync('server.ts', 'utf-8');
const districtsEndpoint = `
  app.get("/api/locations/districts", (req, res) => {
    const state = req.query.state;
    // In the real ASP.NET Core backend, this queries LocationMaster table
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
      // Dynamic fallback for other states simulating backend lookup
      const stateName = STATES_MAP[state] || 'State';
      res.json([
        { code: \`\${state}-01\`, name: \`\${stateName} District 1\` },
        { code: \`\${state}-02\`, name: \`\${stateName} District 2\` },
        { code: \`\${state}-03\`, name: \`\${stateName} District 3\` }
      ]);
    }
  });
`;

if (!serverContent.includes('/api/locations/districts')) {
  serverContent = serverContent.replace('app.post("/api/schemes/match", (req, res) => {', districtsEndpoint + '\n  app.post("/api/schemes/match", (req, res) => {');
  fs.writeFileSync('server.ts', serverContent, 'utf-8');
}

// Patch FindScheme.tsx
let uiContent = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

const newFetchDistricts = `const fetchDistrictsFromBackend = async (stateCode: string): Promise<{code: string, name: string}[]> => {
  try {
    const res = await fetch(\`/api/locations/districts?state=\${stateCode}\`);
    if (!res.ok) throw new Error('Failed to fetch districts');
    return await res.json();
  } catch (error) {
    console.error(error);
    return [];
  }
};`;

uiContent = uiContent.replace(/const fetchDistrictsFromBackend = async \(stateCode: string\): Promise<\{code: string, name: string\}\[]> => \{[\s\S]*?  \};\n/g, newFetchDistricts + '\n');
fs.writeFileSync('src/pages/FindScheme.tsx', uiContent, 'utf-8');
