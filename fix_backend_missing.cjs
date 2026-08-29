const fs = require('fs');
let content = fs.readFileSync('server.ts', 'utf-8');

// Replace the line where we push to ruleComparisons to also include missing rules.
content = content.replace(
  'ruleComparisons: [...evalRes.passed, ...evalRes.failed],',
  "ruleComparisons: [...evalRes.passed, ...evalRes.failed, ...evalRes.missing.map((m: any) => ({ ruleName: m, status: 'Missing' }))],"
);

fs.writeFileSync('server.ts', content, 'utf-8');
