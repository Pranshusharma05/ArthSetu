const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

const target = `<div className="max-w-3xl mx-auto mt-8 mb-24 px-4">
        {schemeResults.recommended.length > 0 ? (`;

const replacement = `<div className="max-w-3xl mx-auto mt-8 mb-24 px-4">
        <button onClick={handleBack} className="flex items-center gap-1.5 text-text-muted font-bold hover:text-primary transition-colors mb-6 text-[14px]">
          <ArrowLeft className="w-4 h-4" />
          Back
        </button>
        {schemeResults.recommended.length > 0 ? (`;

content = content.replace(target, replacement);

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
