import fs from 'fs';

let code = fs.readFileSync('src/pages/FindScheme.tsx', 'utf8');

const startStr = "const renderResult = () => {";
const endStr = "  return (\n    <div className=\"min-h-[calc(100vh-160px)] bg-bg-main w-full py-8 md:py-12\">";

const startIdx = code.indexOf(startStr);
const endIdx = code.indexOf(endStr);

if (startIdx === -1 || endIdx === -1) {
    console.log("Could not find start or end index.");
    process.exit(1);
}

const newRender = `const toggleExpanded = (id: string, type: 'details' | 'notEligible') => {
    if (type === 'details') {
      setExpandedDetails(prev => ({ ...prev, [id]: !prev[id] }));
    } else {
      setExpandedNotEligible(prev => ({ ...prev, [id]: !prev[id] }));
    }
  };

  const isStale = (lastVerified: string) => {
    try {
      const parts = lastVerified.split('-');
      if (parts.length !== 3) return false;
      const verifyDate = new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
      const limit = new Date();
      limit.setMonth(limit.getMonth() - 12);
      return verifyDate < limit;
    } catch {
      return false;
    }
  };

  const renderResult = () => {
    if (!schemeResults) return null;

    return (
      <div className="max-w-3xl mx-auto mt-8 mb-24 px-4">
        {schemeResults.recommended.length > 0 ? (
          <>
            <div className="text-center mb-8">
              <h2 className="text-[14px] font-bold text-text-muted uppercase tracking-wider mb-2">Recommended for You</h2>
            </div>
            {schemeResults.recommended.map((scheme, idx) => (
              <div key={scheme.id} className="bg-white rounded-[24px] border border-secondary/20 shadow-[0_12px_40px_rgb(21,154,140,0.08)] overflow-hidden mb-8">
                <div className="bg-soft-teal p-6 md:p-8 relative">
                  <div className="absolute top-6 right-6 bg-secondary text-white text-[11px] font-bold px-3 py-1 rounded-full tracking-wide">
                    Strong Fit
                  </div>
                  <div className="text-[12px] font-bold text-secondary uppercase tracking-wider mb-2">{scheme.ministry}</div>
                  <h3 className="text-3xl md:text-4xl font-extrabold text-primary mb-2">
                    {scheme.name}
                  </h3>
                  <p className="text-[15px] text-primary/80">
                    {scheme.description}
                  </p>
                </div>

                <div className="p-6 md:p-8 border-b border-gray-100">
                  <h4 className="text-[16px] font-bold text-primary mb-4">Why this matches your profile</h4>
                  <div className="flex flex-col gap-3 mb-4">
                    {scheme.ruleComparisons?.filter((r: any) => r.status === 'Matched').map((rule: any, rIdx: number) => (
                      <div key={rIdx} className="flex items-start gap-2">
                        <CheckCircle2 className="w-4 h-4 text-secondary flex-shrink-0 mt-0.5" />
                        <span className="text-[14px] font-medium text-text-main">{rule.ruleName} matched</span>
                      </div>
                    ))}
                  </div>

                  <button 
                    onClick={() => toggleExpanded(scheme.id, 'details')}
                    className="flex items-center gap-1.5 text-[13px] font-bold text-secondary hover:text-primary transition-colors mt-6"
                  >
                    <span>View eligibility details</span>
                    <ChevronDown className={\`w-4 h-4 transition-transform \${expandedDetails[scheme.id] ? 'rotate-180' : ''}\`} />
                  </button>

                  {expandedDetails[scheme.id] && (
                    <div className="mt-4 border border-gray-100 rounded-xl overflow-hidden bg-gray-50/50">
                      {scheme.ruleComparisons?.map((rule: any, rIdx: number) => (
                        <div key={rIdx} className={\`p-4 \${rIdx !== 0 ? 'border-t border-gray-100' : ''}\`}>
                          <div className="font-bold text-[13px] text-primary mb-2">{rule.ruleName}</div>
                          <div className="grid grid-cols-2 gap-4 text-[13px]">
                            <div>
                              <div className="text-text-muted text-[11px] uppercase tracking-wide mb-0.5">Your Value</div>
                              <div className="font-medium text-text-main">{rule.userValue}</div>
                            </div>
                            <div>
                              <div className="text-text-muted text-[11px] uppercase tracking-wide mb-0.5">Scheme Condition</div>
                              <div className="font-medium text-text-main">{rule.schemeCondition}</div>
                            </div>
                          </div>
                          <div className="mt-2 text-[12px] font-bold flex items-center gap-1.5">
                            {rule.status === 'Matched' && <><span className="w-2 h-2 rounded-full bg-secondary"></span><span className="text-secondary">Matched</span></>}
                            {rule.status === 'Failed' && <><span className="w-2 h-2 rounded-full bg-accent"></span><span className="text-accent">Failed</span></>}
                            {rule.status === 'Missing' && <><span className="w-2 h-2 rounded-full bg-orange-400"></span><span className="text-orange-500">Missing</span></>}
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </div>

                <div className="p-6 md:p-8 border-b border-gray-100 bg-gray-50/30">
                  <h4 className="text-[16px] font-bold text-primary mb-4">Benefit Summary</h4>
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                    <div>
                      <div className="text-[12px] font-bold text-text-muted uppercase tracking-wide mb-1">Benefit Type</div>
                      <div className="text-[16px] font-bold text-primary">{scheme.benefitType}</div>
                    </div>
                    {scheme.subsidy && (
                      <div>
                        <div className="text-[12px] font-bold text-text-muted uppercase tracking-wide mb-1">Subsidy / Coverage</div>
                        <div className="text-[16px] font-bold text-primary">{scheme.subsidy}</div>
                      </div>
                    )}
                  </div>
                </div>

                <div className="bg-gray-50 px-6 py-4 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 text-[12px]">
                  <div className="flex flex-col gap-1">
                    <div className="flex items-center gap-1.5 text-text-main font-semibold">
                      {isStale(scheme.lastVerified) ? (
                        <AlertCircle className="w-4 h-4 text-orange-500" />
                      ) : (
                        <ShieldCheck className="w-4 h-4 text-secondary" />
                      )}
                      <span>Verified: {scheme.source}</span>
                    </div>
                    <span className={\`ml-5.5 \${isStale(scheme.lastVerified) ? 'text-orange-600 font-medium' : 'text-text-muted'}\`}>
                      {isStale(scheme.lastVerified) ? 'Source verification update recommended' : \`Last verified: \${scheme.lastVerified}\`}
                    </span>
                  </div>
                  <a href={\`https://\${scheme.source}\`} target="_blank" rel="noopener noreferrer" className="font-semibold text-primary hover:text-secondary underline underline-offset-2 sm:ml-auto">View Source</a>
                </div>

                <div className="flex flex-col sm:flex-row items-center gap-4 p-6 md:p-8 bg-white border-t border-gray-100">
                  {scheme.applicationMode === 'Online' || !scheme.applicationMode ? (
                    <button className="w-full sm:w-auto flex-1 flex items-center justify-center gap-2 bg-primary text-white px-8 py-4 rounded-[12px] font-bold text-[16px] hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg transition-all">
                      Apply on Official Portal <ArrowRight className="w-5 h-5" />
                    </button>
                  ) : (
                    <button className="w-full sm:w-auto flex-1 flex items-center justify-center bg-white text-primary border border-primary/20 px-8 py-4 rounded-[12px] font-bold text-[16px] hover:border-primary hover:-translate-y-0.5 hover:shadow-sm transition-all shadow-sm">
                      Find Authorised Partner
                    </button>
                  )}
                </div>
              </div>
            ))}
          </>
        ) : (
          <div className="text-center mb-8 py-12 bg-white rounded-[24px] border border-gray-200">
            <h2 className="text-xl font-bold text-primary mb-2">No exact match found</h2>
            <p className="text-text-muted">We couldn't find a strongly matching scheme for your specific combination of inputs.</p>
          </div>
        )}

        {schemeResults.otherEligible.length > 0 && (
          <div>
            <h4 className="text-[16px] font-bold text-primary mb-4">Other Eligible Options</h4>
            {schemeResults.otherEligible.map((scheme: any) => (
              <div key={scheme.id} className="bg-white rounded-xl border border-gray-100 p-4 mb-4 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                <div>
                  <div className="text-[11px] font-bold text-text-muted uppercase tracking-wide mb-1">{scheme.ministry}</div>
                  <div className="font-bold text-primary text-[15px]">{scheme.name}</div>
                  <div className="text-[13px] text-text-muted mt-0.5">{scheme.benefitType}</div>
                </div>
                <button className="text-[13px] font-bold text-secondary hover:text-primary transition-colors flex-shrink-0">View Details</button>
              </div>
            ))}
          </div>
        )}

        {schemeResults.moreInfoNeeded.length > 0 && (
          <div className="border border-gray-100 rounded-xl overflow-hidden bg-white mt-8">
            <button 
              onClick={() => setExpandedMoreInfo(!expandedMoreInfo)}
              className="w-full px-5 py-4 flex items-center justify-between text-[14px] font-bold text-text-main hover:bg-gray-50 transition-colors"
            >
              <span>More Information Needed</span>
              <ChevronDown className={\`w-5 h-5 text-text-muted transition-transform \${expandedMoreInfo ? 'rotate-180' : ''}\`} />
            </button>
            
            {expandedMoreInfo && (
              <div className="px-5 pb-5 pt-1 border-t border-gray-50 space-y-4">
                {schemeResults.moreInfoNeeded.map((scheme: any) => (
                  <div key={scheme.id}>
                    <div className="font-bold text-primary text-[14px] mb-1">{scheme.name}</div>
                    <div className="text-[13px] font-medium text-orange-500 mb-1">
                      Needs: {scheme.ruleComparisons?.filter((r: any) => r.status === 'Missing').map((r: any) => r.ruleName).join(', ')}
                    </div>
                    <div className="text-[13px] text-text-muted leading-relaxed">We need more information to confirm your eligibility for this scheme.</div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {schemeResults.notEligible && schemeResults.notEligible.length > 0 && (
          <div className="border border-gray-100 rounded-xl overflow-hidden bg-white mt-8 mb-8">
            <button 
              onClick={() => setExpandedAlternative(!expandedAlternative)}
              className="w-full px-5 py-4 flex items-center justify-between text-[14px] font-bold text-text-main hover:bg-gray-50 transition-colors"
            >
              <span>Schemes You Do Not Qualify For</span>
              <ChevronDown className={\`w-5 h-5 text-text-muted transition-transform \${expandedAlternative ? 'rotate-180' : ''}\`} />
            </button>
            
            {expandedAlternative && (
              <div className="px-5 pb-5 pt-1 border-t border-gray-50 space-y-4">
                {schemeResults.notEligible.map((scheme: any) => (
                  <div key={scheme.id} className="border-b border-gray-50 pb-4 last:border-0 last:pb-0">
                    <div className="font-bold text-primary text-[14px] mb-1">{scheme.name}</div>
                    
                    <button 
                      onClick={() => toggleExpanded(scheme.id, 'notEligible')}
                      className="flex items-center gap-1.5 text-[12px] font-bold text-accent hover:text-primary transition-colors mt-2 mb-3"
                    >
                      <span>Why this scheme did not match</span>
                      <ChevronDown className={\`w-3 h-3 transition-transform \${expandedNotEligible[scheme.id] ? 'rotate-180' : ''}\`} />
                    </button>

                    {expandedNotEligible[scheme.id] && (
                      <div className="bg-red-50/50 rounded-lg p-3 border border-red-100/50">
                        {scheme.ruleComparisons?.filter((r: any) => r.status === 'Failed').map((rule: any, rIdx: number) => (
                          <div key={rIdx} className={\`\${rIdx !== 0 ? 'mt-3 pt-3 border-t border-red-100/50' : ''}\`}>
                            <div className="flex items-start gap-2">
                              <XCircle className="w-4 h-4 text-accent flex-shrink-0 mt-0.5" />
                              <div>
                                <div className="text-[13px] font-bold text-accent mb-1">{rule.ruleName} requirement not matched</div>
                                <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 mt-2">
                                  <div className="text-[12px]">
                                    <span className="text-text-muted uppercase tracking-wide text-[10px] block mb-0.5">Your Profile</span>
                                    <span className="font-medium text-text-main">{rule.userValue}</span>
                                  </div>
                                  <div className="text-[12px]">
                                    <span className="text-text-muted uppercase tracking-wide text-[10px] block mb-0.5">Scheme Requirement</span>
                                    <span className="font-medium text-text-main">{rule.schemeCondition}</span>
                                  </div>
                                </div>
                              </div>
                            </div>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        <div className="flex flex-col sm:flex-row items-center gap-4 mt-8 mb-16">
          <button onClick={() => { setStep('intro'); setStepHistory([]); }} className="w-full sm:w-auto flex-1 flex items-center justify-center bg-white text-primary border border-primary/20 px-8 py-4 rounded-[12px] font-bold text-[16px] hover:border-primary hover:-translate-y-0.5 hover:shadow-sm transition-all shadow-sm">
            Start New Search
          </button>
        </div>
      </div>
    );
  };
`

code = code.substring(0, startIdx) + newRender + code.substring(endIdx);
fs.writeFileSync('src/pages/FindScheme.tsx', code);
console.log("Replaced successfully!");
