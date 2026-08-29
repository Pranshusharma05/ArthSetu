import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { ShieldCheck, Info, Calculator, IndianRupee, Clock, ArrowLeft } from 'lucide-react';

export function LoanPlanner() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const initialSchemeId = searchParams.get('schemeId');
  
  const [schemes, setSchemes] = useState<any[]>([]);
  const [benefitComponents, setBenefitComponents] = useState<any[]>([]);
  const [provenances, setProvenances] = useState<any[]>([]);
  
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  const [selectedSchemeId, setSelectedSchemeId] = useState<string>(initialSchemeId || '');
  const [requestedLoan, setRequestedLoan] = useState<number>(100000);
  const [projectCost, setProjectCost] = useState<number>(0);
  const [sector, setSector] = useState<string>('Service');
  const [planningTenure, setPlanningTenure] = useState<number>(60); // months

  useEffect(() => {
    const fetchLoans = async () => {
      try {
        const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
        const res = await fetch(`${baseUrl}/api/schemes/loans`);
        if (!res.ok) throw new Error('API unavailable');
        const data = await res.json();
        setSchemes(data.schemes);
        setBenefitComponents(data.benefitComponents);
        setProvenances(data.provenances);
      } catch (err) {
        setError('Service unavailable');
      } finally {
        setLoading(false);
      }
    };
    fetchLoans();
  }, []);

  if (loading) return <div className="p-10 text-center">Loading verified government loans...</div>;
  if (error) return <div className="p-10 text-center text-red-500 font-bold">{error}</div>;

  const getProvenanceValue = (entityId: string, fieldName: string) => {
    const prov = provenances.find(p => p.entityId === entityId && p.fieldName === fieldName);
    return prov ? prov.rawValue : null;
  };

  const getNumericValue = (val: string | null) => {
    if (!val) return null;
    const match = val.match(/[\d\.]+/);
    return match ? parseFloat(match[0]) : null;
  };

  const selectedScheme = schemes.find(s => s.id === selectedSchemeId);
  const componentsForScheme = benefitComponents.filter(c => c.schemeId === selectedSchemeId);

  const calculateTerms = () => {
    if (!selectedScheme) return null;

    let interestRateStr = getProvenanceValue(selectedScheme.id, 'InterestRate');
    let interestRate = getNumericValue(interestRateStr);
    
    let maxLoanStr = getProvenanceValue(selectedScheme.id, 'LoanMax');
    let maxLoan = maxLoanStr?.toLowerCase().includes('lakh') ? getNumericValue(maxLoanStr) * 100000 : getNumericValue(maxLoanStr);
    
    let tenureStr = getProvenanceValue(selectedScheme.id, 'Tenure');
    let maxTenureMonths = tenureStr?.toLowerCase().includes('year') ? getNumericValue(tenureStr) * 12 : getNumericValue(tenureStr);

    let isPMMY = selectedScheme.id === 'pmmy';
    let isPMEGP = selectedScheme.id === 'pmegp';
    let isVidyalaxmi = selectedScheme.id === 'pm-vidyalaxmi';

    if (isPMMY) {
       if (requestedLoan <= 50000) maxLoan = 50000;
       else if (requestedLoan <= 500000) maxLoan = 500000;
       else if (requestedLoan <= 1000000) maxLoan = 1000000;
       else maxLoan = 2000000; // Tarun Plus
       if (requestedLoan > 1000000 && requestedLoan <= 2000000) {
          // Tarun Plus prerequisite check (simplified)
          // Assume calculable for now, but mark it
       }
    }

    if (isPMEGP) {
       maxLoan = sector === 'Manufacturing' ? 5000000 : 2000000;
    }

    let calculable = true;
    let missingFields = [];

    if (!interestRateStr) { calculable = false; missingFields.push('Interest Rate'); }
    if (!tenureStr) { calculable = false; missingFields.push('Tenure'); }

    let principal = Math.min(requestedLoan, maxLoan || requestedLoan);
    let emi = 0;
    let totalInterest = 0;
    let totalRepayment = 0;

    let amortization = [];
    if (calculable && interestRate) {
      let r = (interestRate / 12) / 100;
      let n = Math.min(planningTenure, maxTenureMonths || planningTenure);
      if (r > 0) {
        emi = (principal * r * Math.pow(1 + r, n)) / (Math.pow(1 + r, n) - 1);
        totalRepayment = emi * n;
        totalInterest = totalRepayment - principal;
        
        let balance = principal;
        for (let i = 1; i <= Math.min(n, 12); i++) { // show first year
           let iterInterest = balance * r;
           let iterPrincipal = emi - iterInterest;
           balance -= iterPrincipal;
           amortization.push({ month: i, emi, principal: iterPrincipal, interest: iterInterest, balance });
        }
      }
    }

    return {
      interestRateStr, interestRate, maxLoan, maxTenureMonths, calculable, missingFields,
      principal, emi, totalRepayment, totalInterest, amortization
    };
  };

  const terms = calculateTerms();

  return (
    <div className="max-w-6xl mx-auto p-6 mt-10">
      <div className="flex items-center gap-4 mb-8">
        <button onClick={() => navigate(-1)} className="p-2 bg-gray-100 rounded-full hover:bg-gray-200">
          <ArrowLeft size={20} />
        </button>
        <h1 className="text-3xl font-bold text-gray-800">ArthSetu Loan Planner</h1>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div className="md:col-span-1 space-y-6">
          <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-100">
            <h2 className="font-bold text-gray-800 mb-4">Select Verified Scheme</h2>
            <select 
              value={selectedSchemeId} 
              onChange={e => setSelectedSchemeId(e.target.value)}
              className="w-full p-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-primary focus:outline-none"
            >
              <option value="">-- Choose a Government Scheme --</option>
              {schemes.map(s => (
                <option key={s.id} value={s.id}>{s.name} ({s.owningAuthority})</option>
              ))}
            </select>

            {selectedSchemeId && (
              <div className="mt-6 space-y-4">
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Requested Loan Amount (?)</label>
                  <input type="number" value={requestedLoan} onChange={e => setRequestedLoan(Number(e.target.value))} className="w-full p-2 border border-gray-200 rounded-lg" />
                </div>
                {selectedScheme?.id === 'pmegp' && (
                  <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-1">Sector</label>
                    <select value={sector} onChange={e => setSector(e.target.value)} className="w-full p-2 border border-gray-200 rounded-lg">
                      <option value="Service">Service</option>
                      <option value="Manufacturing">Manufacturing</option>
                    </select>
                  </div>
                )}
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-1">Planning Tenure (Months)</label>
                  <input type="number" value={planningTenure} onChange={e => setPlanningTenure(Number(e.target.value))} className="w-full p-2 border border-gray-200 rounded-lg" />
                </div>
              </div>
            )}
          </div>
        </div>

        <div className="md:col-span-2">
          {selectedScheme && terms ? (
            <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
              <div className="bg-soft-teal p-6 border-b border-gray-100">
                <div className="flex items-center gap-2 text-primary font-bold mb-2">
                  <ShieldCheck size={20} />
                  <span>Verified Source</span>
                </div>
                <h2 className="text-2xl font-bold text-gray-800">{selectedScheme.name}</h2>
                <p className="text-sm text-gray-600 mt-2">Source: {selectedScheme.officialSourceUrl || 'Official Portal'} | Last Verified: {new Date(selectedScheme.lastVerified).toLocaleDateString()}</p>
              </div>

              <div className="p-6">
                {!terms.calculable ? (
                  <div className="bg-amber-50 border border-amber-200 p-4 rounded-lg flex gap-3 text-amber-800">
                    <Info className="flex-shrink-0" />
                    <div>
                      <h4 className="font-bold">Detailed EMI unavailable until verified repayment terms are available.</h4>
                      <p className="text-sm mt-1">Missing: {terms.missingFields.join(', ')}</p>
                      {terms.interestRateStr?.toLowerCase().includes('channel') && (
                        <p className="text-sm mt-1">Interest rate depends on implementing/channel partner.</p>
                      )}
                    </div>
                  </div>
                ) : (
                  <>
                    <div className="grid grid-cols-2 gap-4 mb-8">
                      <div className="p-4 bg-gray-50 rounded-lg border border-gray-100">
                        <div className="text-sm text-gray-500 font-medium">Estimated Eligible Finance</div>
                        <div className="text-2xl font-bold text-gray-800">?{terms.principal.toLocaleString()}</div>
                        {terms.maxLoan && <div className="text-xs text-gray-400 mt-1">Official maximum finance: ?{terms.maxLoan.toLocaleString()}</div>}
                      </div>
                      <div className="p-4 bg-gray-50 rounded-lg border border-gray-100">
                        <div className="text-sm text-gray-500 font-medium">Monthly EMI</div>
                        <div className="text-2xl font-bold text-primary">?{Math.round(terms.emi).toLocaleString()}</div>
                        <div className="text-xs text-gray-400 mt-1">Official beneficiary interest: {terms.interestRateStr || 'N/A'}</div>
                      </div>
                    </div>

                    <div className="border-t border-gray-100 pt-6">
                      <h3 className="font-bold text-gray-800 mb-4 flex items-center gap-2">
                        <Calculator size={18} /> Repayment Summary
                      </h3>
                      <div className="space-y-3">
                        <div className="flex justify-between items-center py-2 border-b border-gray-50">
                          <span className="text-gray-600">Total Principal</span>
                          <span className="font-bold text-gray-800">₹{Math.round(terms.principal).toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between items-center py-2 border-b border-gray-50">
                          <span className="text-gray-600">Total Interest</span>
                          <span className="font-bold text-gray-800">₹{Math.round(terms.totalInterest).toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between items-center py-2 border-b border-gray-50">
                          <span className="text-gray-600">Total Repayment</span>
                          <span className="font-bold text-gray-800">₹{Math.round(terms.totalRepayment).toLocaleString()}</span>
                        </div>
                      </div>
                    </div>
                    {terms.amortization.length > 0 && (
                      <div className="border-t border-gray-100 pt-6 mt-6">
                        <h3 className="font-bold text-gray-800 mb-4 flex items-center gap-2">
                          <Clock size={18} /> First Year Amortization Schedule
                        </h3>
                        <div className="overflow-x-auto">
                          <table className="w-full text-sm text-left border-collapse">
                            <thead>
                              <tr className="bg-gray-50 text-gray-600">
                                <th className="p-2 border">Month</th>
                                <th className="p-2 border">EMI</th>
                                <th className="p-2 border">Principal</th>
                                <th className="p-2 border">Interest</th>
                                <th className="p-2 border">Balance</th>
                              </tr>
                            </thead>
                            <tbody>
                              {terms.amortization.map(row => (
                                <tr key={row.month} className="border-b">
                                  <td className="p-2 border">{row.month}</td>
                                  <td className="p-2 border">₹{Math.round(row.emi).toLocaleString()}</td>
                                  <td className="p-2 border">₹{Math.round(row.principal).toLocaleString()}</td>
                                  <td className="p-2 border">₹{Math.round(row.interest).toLocaleString()}</td>
                                  <td className="p-2 border">₹{Math.round(row.balance).toLocaleString()}</td>
                                </tr>
                              ))}
                            </tbody>
                          </table>
                        </div>
                      </div>
                    )}
                  </>
                )}
              </div>
            </div>
          ) : (
            <div className="h-full flex flex-col items-center justify-center text-gray-400 p-10 bg-gray-50 rounded-xl border border-dashed border-gray-200">
              <Calculator size={48} className="mb-4 opacity-50" />
              <p>Select a verified government scheme to calculate terms.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

