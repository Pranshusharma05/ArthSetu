const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

const replacement = `      <div className="mt-8 flex flex-col items-end gap-3">
        {(!formData.beneficiaryType || !isValidAge || !formData.gender || !formData.category) && (
          <div className="text-[13px] text-red-500 font-medium bg-red-50 px-4 py-2 rounded-lg border border-red-100 flex items-center gap-2">
            <AlertCircle className="w-4 h-4" />
            <span>
              Missing required fields: 
              {[
                !formData.beneficiaryType && 'Beneficiary Type',
                !isValidAge && 'Valid Date of Birth',
                !formData.gender && 'Gender',
                !formData.category && 'Social Category'
              ].filter(Boolean).join(', ')}
            </span>
          </div>
        )}
        <button 
          disabled={!formData.beneficiaryType || !isValidAge || !formData.gender || !formData.category}
          onClick={() => handleNext('financial')}
          className={\`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center justify-center gap-2 \${formData.beneficiaryType && isValidAge && formData.gender && formData.category ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}\`}
        >
          Continue <ArrowRight className="w-5 h-5" />
        </button>
      </div>`;

content = content.replace(/<div className="mt-10 flex justify-end items-center">[\s\S]*?<\/div>\s*<\/div>\s*\);\s*};\s*const renderFinancial =/m, replacement + '\n    </div>\n  );\n  };\n\n  const renderFinancial =');

// Let's also remove the extra margin at the top level of renderAbout if it exists
content = content.replace(/<div className="max-w-2xl mx-auto mt-8 mb-24 px-4">/, '<div className="max-w-2xl mx-auto mt-8 mb-12 px-4">');

// Ensure gender is initialized and correctly controlled
content = content.replace(/value={formData\.gender}/, 'value={formData.gender || ""}');

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
