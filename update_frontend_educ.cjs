const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

// Replace fetchDynamicQuestions
const oldFetch = /const fetchDynamicQuestions = async \(data: any\): Promise<DynamicQuestion\[\]> => \{[\s\S]*?return questions;\n\};/;

const newFetch = `const fetchDynamicQuestions = async (data: any): Promise<DynamicQuestion[]> => {
  try {
    const res = await fetch('/api/schemes/dynamic-questions', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });
    if (!res.ok) throw new Error('Failed to fetch dynamic questions');
    return await res.json();
  } catch (error) {
    console.error(error);
    return [];
  }
};`;

content = content.replace(oldFetch, newFetch);

// The result display logic needs to render More Info Needed properly.
// The frontend has `schemeResults.recommended`, `schemeResults.partialMatches` etc.
// Wait, my backend now returns `recommended`, `otherEligible`, `moreInfoNeeded`, `notEligible`.
// I need to find where `partialMatches` was used in `FindScheme.tsx` and replace it with `moreInfoNeeded` and `otherEligible`.

content = content.replace(/schemeResults\.partialMatches/g, 'schemeResults.moreInfoNeeded');
// Also change the title from "Other Eligible Options" or "Partial Matches" to "More Information Needed"
// Let's replace "Partial Matches" with "More Information Needed" if it exists.
content = content.replace(/>Partial Matches</g, '>More Information Needed<');

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
