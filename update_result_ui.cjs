const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

const injection = `
                    {scheme.officialSource && (
                      <div>
                        <div className="text-[12px] font-bold text-text-muted uppercase tracking-wide mb-1">Official Source</div>
                        <div className="text-[14px] font-bold text-primary flex items-center gap-1.5">
                          {scheme.officialSource}
                          {scheme.verificationStatus === 'Verified' && <CheckCircle2 className="w-3.5 h-3.5 text-secondary" />}
                        </div>
                        {scheme.lastVerified && <div className="text-[11px] text-text-muted mt-0.5">Last Verified: {scheme.lastVerified}</div>}
                      </div>
                    )}
                    {scheme.applicationRoute && (
                      <div className="sm:col-span-2 mt-2 bg-blue-50/50 p-4 rounded-xl border border-blue-100">
                        <div className="text-[12px] font-bold text-blue-600 uppercase tracking-wide mb-1">Application Route</div>
                        <div className="text-[14px] font-bold text-primary">{scheme.applicationRoute}</div>
                      </div>
                    )}
`;

content = content.replace(
  /\{scheme\.subsidy && \([\s\S]*?<\/div>\s*\)\}/,
  (match) => match + injection
);

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
