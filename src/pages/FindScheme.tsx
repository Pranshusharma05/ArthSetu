import React, { useState } from 'react';
import { ArrowRight, XCircle, AlertCircle, ArrowLeft, Briefcase, GraduationCap, Info, MapPin, ShieldCheck, ChevronDown, ChevronUp, Wrench, Home, Heart, Landmark, Shield, Monitor, Users, Trophy, Bus, Map, Droplet, Baby, Search } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

type Step = 'intro' | 'purpose' | 'about' | 'financial' | 'specifics' | 'location' | 'review' | 'processing' | 'result';

const PURPOSES = [
  { id: 'Agriculture', icon: Map, title: 'Agriculture, Rural & Environment', desc: 'Farming, dairy, or agriculture activity' },
  { id: 'Banking', icon: Landmark, title: 'Banking, Financial Services & Insurance', desc: 'Financial support and insurance schemes' },
  { id: 'Business', icon: Briefcase, title: 'Business & Entrepreneurship', desc: 'Business, services, or manufacturing' },
  { id: 'Education', icon: GraduationCap, title: 'Education & Learning', desc: 'Higher education or professional courses' },
  { id: 'Health', icon: Heart, title: 'Health & Wellness', desc: 'Healthcare and medical assistance' },
  { id: 'Housing', icon: Home, title: 'Housing & Shelter', desc: 'Home construction or improvement' },
  { id: 'PublicSafety', icon: Shield, title: 'Public Safety, Law & Justice', desc: 'Legal and public safety schemes' },
  { id: 'Science', icon: Monitor, title: 'Science, IT & Communications', desc: 'Technology and science-related support' },
  { id: 'Skills', icon: Wrench, title: 'Skills & Employment', desc: 'Vocational training and skill development' },
  { id: 'SocialWelfare', icon: Users, title: 'Social Welfare & Empowerment', desc: 'Support for marginalized sections' },
  { id: 'Sports', icon: Trophy, title: 'Sports & Culture', desc: 'Athletics and cultural promotions' },
  { id: 'Transport', icon: Bus, title: 'Transport & Infrastructure', desc: 'Roads, transport and logistics' },
  { id: 'Travel', icon: MapPin, title: 'Travel & Tourism', desc: 'Hospitality and tourism support' },
  { id: 'Utility', icon: Droplet, title: 'Utility & Sanitation', desc: 'Water, sanitation, and utilities' },
  { id: 'WomenChild', icon: Baby, title: 'Women & Child', desc: 'Schemes dedicated to women and children' }
];

const ACTIVITY_CATEGORIES = [
  { id: 'Agriculture', title: 'Agriculture' }, { id: 'Dairy', title: 'Dairy / Livestock' },
  { id: 'Poultry', title: 'Poultry' }, { id: 'Fisheries', title: 'Fisheries' },
  { id: 'Food Processing', title: 'Food Processing' }, { id: 'Handloom', title: 'Handloom' },
  { id: 'Handicrafts', title: 'Handicrafts' }, { id: 'Textiles', title: 'Textiles / Garments' },
  { id: 'Manufacturing', title: 'Manufacturing' }, { id: 'Trading', title: 'Trading / Retail' },
  { id: 'Professional', title: 'Professional Services' }, { id: 'Repair', title: 'Repair & Maintenance' },
  { id: 'Transport', title: 'Transport / Logistics' }, { id: 'Tourism', title: 'Tourism / Hospitality' },
  { id: 'Digital', title: 'Digital / IT' }, { id: 'Construction', title: 'Construction' },
  { id: 'SelfEmployment', title: 'Self-Employment' }, { id: 'SHG', title: 'SHG / Group Enterprise' },
  { id: 'Startup', title: 'Startup' }, { id: 'Other', title: 'Other' }
];

const STATES = [
  { code: 'AN', name: 'Andaman and Nicobar Islands' }, { code: 'AP', name: 'Andhra Pradesh' },
  { code: 'AR', name: 'Arunachal Pradesh' }, { code: 'AS', name: 'Assam' },
  { code: 'BR', name: 'Bihar' }, { code: 'CH', name: 'Chandigarh' },
  { code: 'CT', name: 'Chhattisgarh' }, { code: 'DN', name: 'Dadra and Nagar Haveli and Daman and Diu' },
  { code: 'DL', name: 'Delhi' }, { code: 'GA', name: 'Goa' },
  { code: 'GJ', name: 'Gujarat' }, { code: 'HR', name: 'Haryana' },
  { code: 'HP', name: 'Himachal Pradesh' }, { code: 'JK', name: 'Jammu and Kashmir' },
  { code: 'JH', name: 'Jharkhand' }, { code: 'KA', name: 'Karnataka' },
  { code: 'KL', name: 'Kerala' }, { code: 'LA', name: 'Ladakh' },
  { code: 'LD', name: 'Lakshadweep' }, { code: 'MP', name: 'Madhya Pradesh' },
  { code: 'MH', name: 'Maharashtra' }, { code: 'MN', name: 'Manipur' },
  { code: 'ML', name: 'Meghalaya' }, { code: 'MZ', name: 'Mizoram' },
  { code: 'NL', name: 'Nagaland' }, { code: 'OR', name: 'Odisha' },
  { code: 'PY', name: 'Puducherry' }, { code: 'PB', name: 'Punjab' },
  { code: 'RJ', name: 'Rajasthan' }, { code: 'SK', name: 'Sikkim' },
  { code: 'TN', name: 'Tamil Nadu' }, { code: 'TG', name: 'Telangana' },
  { code: 'TR', name: 'Tripura' }, { code: 'UP', name: 'Uttar Pradesh' },
  { code: 'UT', name: 'Uttarakhand' }, { code: 'WB', name: 'West Bengal' }
];

const fetchDistrictsFromBackend = async (stateCode: string): Promise<{code: string, name: string}[]> => {
  try {
    const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
    const res = await fetch(`${baseUrl}/api/locations/districts?state=${stateCode}`);
    if (!res.ok) throw new Error('Failed to fetch districts');
    return await res.json();
  } catch (error) { console.error(error); return []; }
};

interface DynamicQuestion {
  id: string;
  type: 'single_choice' | 'numeric' | 'currency' | 'text' | 'yes_no' | 'taxonomy';
  label: string;
  helpText?: string;
  options?: { label: string; value: string }[];
}

const fetchDynamicQuestions = async (data: any): Promise<DynamicQuestion[]> => {
  try {
    const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
    const res = await fetch(`${baseUrl}/api/schemes/dynamic-questions`, {
      method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data)
    });
    if (!res.ok) throw new Error('Failed to fetch dynamic questions');
    return await res.json();
  } catch (error) { console.error(error); return []; }
};

const TaxonomySelector = ({ value, onChange }: { value: any, onChange: (val: any) => void }) => (
  <div className="space-y-4">
    <select value={value.activity} onChange={(e) => onChange({ ...value, activity: e.target.value })}
      className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 px-4 text-[16px] font-bold text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary">
      <option value="">Select an activity</option>
      {ACTIVITY_CATEGORIES.map(c => <option key={c.id} value={c.id}>{c.title}</option>)}
    </select>
    {value.activity === 'Other' && (
      <input type="text" placeholder="Please specify" value={value.customActivityText || ''}
        onChange={(e) => onChange({ ...value, customActivityText: e.target.value })}
        className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 px-4 text-[16px] font-bold text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary" />
    )}
  </div>
);

export function FindScheme() {
  const navigate = useNavigate();
  const [step, setStep] = useState<Step>('intro');
  const [formData, setFormData] = useState<any>({
    purpose: '', category: '', dob: '', income: '', state: '', stateName: '',
    district: '', districtName: '', beneficiaryType: 'Myself', dynamicAnswers: {}
  });
  const [availableDistricts, setAvailableDistricts] = useState<{code: string, name: string}[]>([]);
  const [dynamicQuestions, setDynamicQuestions] = useState<DynamicQuestion[]>([]);
  const [isFetchingQuestions, setIsFetchingQuestions] = useState(false);
  const [schemeResults, setSchemeResults] = useState<any>(null);
  const [expandedNotEligible, setExpandedNotEligible] = useState<Record<string, boolean>>({});
  const [showAllCategories, setShowAllCategories] = useState(false);
  const [showOptionalAttributes, setShowOptionalAttributes] = useState(false);
  const [isLoadingDistricts, setIsLoadingDistricts] = useState(false);
  const [expandedDetails, setExpandedDetails] = useState<Record<string, boolean>>({});
  const [expandedMoreInfo, setExpandedMoreInfo] = useState(false);
  const [expandedAlternative, setExpandedAlternative] = useState(false);
  const [stepHistory, setStepHistory] = useState<Step[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [currentFilter, setCurrentFilter] = useState("All");
  const ITEMS_PER_PAGE = 10;

  const handleStateChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const code = e.target.value;
    const name = STATES.find(s => s.code === code)?.name || '';
    setFormData({ ...formData, state: code, stateName: name, district: '', districtName: '', dynamicAnswers: {} });
    setIsLoadingDistricts(true);
    const districts = await fetchDistrictsFromBackend(code);
    setIsLoadingDistricts(false);
    setAvailableDistricts(districts);
  };

  const handleDistrictChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const code = e.target.value;
    const name = availableDistricts.find(d => d.code === code)?.name || '';
    setFormData({ ...formData, district: code, districtName: name, dynamicAnswers: {} });
  };

  const getStepNumber = () => {
    switch (step) {
      case 'purpose': return 1; case 'about': return 2; case 'financial': return 3;
      case 'location': return 4; case 'specifics': return 5; case 'review': return 6;
      default: return 0;
    }
  };

  const handleNext = (nextStep: Step) => { window.scrollTo(0, 0); setStepHistory(prev => [...prev, step]); setStep(nextStep); };

  const handleBack = () => {
    if (step === 'purpose') {
      if (window.history.state && window.history.state.idx > 0) { navigate(-1); } else { navigate('/'); }
      return;
    }
    setStepHistory(prev => {
      let newHistory = [...prev];
      let previousStep = newHistory.pop();
      while (previousStep === 'processing' && newHistory.length > 0) { previousStep = newHistory.pop(); }
      if (previousStep) { setStep(previousStep); window.scrollTo(0, 0); }
      return newHistory;
    });
  };

  const toggleExpanded = (id: string, section: string) => {
    if (section === 'notEligible') setExpandedNotEligible(prev => ({ ...prev, [id]: !prev[id] }));
    else if (section === 'details') setExpandedDetails(prev => ({ ...prev, [id]: !prev[id] }));
  };

  const renderIntro = () => (
    <div className="max-w-2xl mx-auto text-center mt-12 mb-24 px-4">
      <div className="inline-flex items-center gap-2 bg-soft-teal text-secondary px-3 py-1.5 rounded-full font-semibold text-xs tracking-wide mb-6">SMART SCHEME RECOMMENDER</div>
      <h1 className="text-3xl md:text-5xl font-extrabold text-primary leading-tight mb-6">Let's Find the Right Scheme for You</h1>
      <p className="text-lg text-text-muted leading-relaxed mb-8">Answer a few simple questions about your need. ArthSetu will check applicable scheme conditions and show the most suitable options with clear reasons.</p>
      <div className="flex flex-col items-center gap-3 mb-10">
        <span className="text-[14px] font-semibold text-text-main bg-gray-100 px-4 py-2 rounded-full">Takes about 2 minutes</span>
        <div className="flex items-center gap-1.5 text-text-muted text-[13px]"><ShieldCheck className="w-4 h-4 text-secondary" /><span>No documents required at this stage</span></div>
      </div>
      <button onClick={() => handleNext('purpose')} className="bg-primary text-white px-10 py-4 rounded-[12px] font-bold text-[16px] hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg transition-all flex items-center justify-center gap-2 mx-auto w-full sm:w-auto">Start <ArrowRight className="w-5 h-5" /></button>
    </div>
  );

  const renderPurpose = () => (
    <div className="max-w-3xl mx-auto mt-8 mb-24 px-4">
      <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-8 text-center">What do you need support for?</h2>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
        {PURPOSES.map((item) => (
          <div key={item.id} onClick={() => setFormData({ ...formData, purpose: item.id })}
            className={`p-5 rounded-[16px] border-2 cursor-pointer transition-all hover:-translate-y-0.5 hover:shadow-md ${formData.purpose === item.id ? 'border-secondary bg-soft-teal' : 'border-gray-100 bg-white hover:border-gray-200'}`}>
            <item.icon className={`w-6 h-6 mb-3 ${formData.purpose === item.id ? 'text-secondary' : 'text-text-muted'}`} />
            <div className={`font-bold text-[14px] mb-1 ${formData.purpose === item.id ? 'text-secondary' : 'text-primary'}`}>{item.title}</div>
            <div className="text-[12px] text-text-muted leading-relaxed">{item.desc}</div>
          </div>
        ))}
      </div>
      {formData.purpose === 'Other' && (
        <input type="text" placeholder="Please describe your need" value={formData.customPurposeText || ''} onChange={(e) => setFormData({ ...formData, customPurposeText: e.target.value })} className="w-full bg-white border border-gray-200 rounded-xl py-3.5 px-4 text-[15px] text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary mb-6" />
      )}
      <div className="mt-12 flex justify-end">
        <button disabled={!formData.purpose || (formData.purpose === 'Other' && !formData.customPurposeText?.trim())} onClick={() => handleNext('about')}
          className={`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center justify-center gap-2 w-full sm:w-auto ${(formData.purpose && (formData.purpose !== 'Other' || formData.customPurposeText?.trim())) ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}`}>
          Continue <ArrowRight className="w-5 h-5" />
        </button>
      </div>
    </div>
  );

  const renderAbout = () => {
    const dob = formData.dob ? new Date(formData.dob) : null;
    const age = dob ? Math.floor((Date.now() - dob.getTime()) / (1000 * 60 * 60 * 24 * 365.25)) : null;
    const isValidAge = age !== null && age >= 18 && age <= 70;
    const CATEGORIES = [
      { id: 'SC', label: 'Scheduled Caste (SC)' }, { id: 'ST', label: 'Scheduled Tribe (ST)' },
      { id: 'OBC', label: 'Other Backward Class (OBC)' }, { id: 'General', label: 'General / Open Category' },
      { id: 'EWS', label: 'Economically Weaker Section (EWS)' }, { id: 'Minority', label: 'Minority Community' },
    ];
    return (
      <div className="max-w-2xl mx-auto mt-8 mb-24 px-4">
        <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-8 text-center">Tell us about yourself</h2>
        <div className="bg-white rounded-[20px] p-6 md:p-8 border border-gray-100 shadow-sm flex flex-col gap-6">
          <div>
            <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">I am applying for</label>
            <div className="flex flex-wrap gap-2">
              {['Myself', 'Business', 'Family Member'].map(val => (
                <button key={val} onClick={() => setFormData({ ...formData, beneficiaryType: val })}
                  className={`px-4 py-2.5 rounded-full text-[13px] font-semibold border transition-all ${formData.beneficiaryType === val ? 'bg-secondary text-white border-secondary shadow-sm' : 'bg-white text-text-muted border-gray-200 hover:border-gray-300'}`}>{val}</button>
              ))}
            </div>
          </div>
          <div>
            <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Date of Birth</label>
            <input type="date" value={formData.dob} onChange={(e) => setFormData({ ...formData, dob: e.target.value })}
              max={new Date(Date.now() - 18 * 365.25 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]}
              className="w-full sm:w-1/2 bg-white border border-gray-200 rounded-xl py-3 px-4 text-[14px] text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary" />
            {formData.dob && !isValidAge && <p className="text-[12px] text-red-500 mt-1">Age must be between 18 and 70 years.</p>}
          </div>
          <div>
            <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Gender</label>
            <div className="flex flex-wrap gap-2">
              {['Male', 'Female', 'Other'].map(val => (
                <button key={val} onClick={() => setFormData({ ...formData, gender: val })}
                  className={`px-4 py-2.5 rounded-full text-[13px] font-semibold border transition-all ${formData.gender === val ? 'bg-secondary text-white border-secondary shadow-sm' : 'bg-white text-text-muted border-gray-200 hover:border-gray-300'}`}>{val}</button>
              ))}
            </div>
          </div>
          <div>
            <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Social Category</label>
            <div className="flex flex-wrap gap-2">
              {(showAllCategories ? CATEGORIES : CATEGORIES.slice(0, 4)).map(cat => (
                <button key={cat.id} onClick={() => setFormData({ ...formData, category: cat.id })}
                  className={`px-4 py-2.5 rounded-full text-[13px] font-semibold border transition-all ${formData.category === cat.id ? 'bg-secondary text-white border-secondary shadow-sm' : 'bg-white text-text-muted border-gray-200 hover:border-gray-300'}`}>{cat.label}</button>
              ))}
              {!showAllCategories && <button onClick={() => setShowAllCategories(true)} className="px-4 py-2.5 rounded-full text-[13px] font-semibold border border-dashed border-gray-300 text-text-muted hover:border-gray-400 transition-all">+ More</button>}
            </div>
          </div>
          <div>
            <button onClick={() => setShowOptionalAttributes(!showOptionalAttributes)} className="flex items-center gap-2 text-[13px] font-semibold text-secondary hover:text-primary transition-colors">
              {showOptionalAttributes ? <ChevronUp className="w-4 h-4" /> : <ChevronDown className="w-4 h-4" />}
              Optional: Additional profile details (may unlock more schemes)
            </button>
          </div>
          {showOptionalAttributes && (
            <div className="flex flex-col gap-5 pt-2 border-t border-gray-100">
              {formData.category === 'Minority' && (
                <div>
                  <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Minority Community</label>
                  <select value={formData.minorityCommunity} onChange={(e) => setFormData({ ...formData, minorityCommunity: e.target.value })} className="w-full sm:w-1/2 bg-white border border-gray-200 rounded-xl py-3 px-4 text-[14px] text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary appearance-none">
                    <option value="" disabled>Select Community</option>
                    <option value="Muslim">Muslim</option><option value="Christian">Christian</option>
                    <option value="Sikh">Sikh</option><option value="Buddhist">Buddhist</option>
                    <option value="Parsi">Parsi</option><option value="Jain">Jain</option>
                  </select>
                </div>
              )}
              <div>
                <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Person with Disability (PwD)</label>
                <div className="flex flex-wrap gap-2">
                  {['Yes', 'No'].map(val => (
                    <button key={val} onClick={() => {
                      const isYes = val === 'Yes';
                      setFormData((prev: any) => {
                        const newDynamic = { ...prev.dynamicAnswers };
                        if (!isYes) delete newDynamic.DisabilityPercentage;
                        return { ...prev, isPwD: isYes, dynamicAnswers: newDynamic };
                      });
                    }}
                      className={`px-4 py-2.5 rounded-full text-[13px] font-semibold border transition-all ${formData.isPwD === (val === 'Yes') ? 'bg-secondary text-white border-secondary shadow-sm' : 'bg-white text-text-muted border-gray-200 hover:border-gray-300'}`}>{val}</button>
                  ))}
                </div>
              </div>
              <div>
                <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Ex-Serviceman</label>
                <div className="flex flex-wrap gap-2">
                  {['Yes', 'No'].map(val => (
                    <button key={val} onClick={() => setFormData((prev: any) => ({ ...prev, isExServiceman: val === 'Yes' }))}
                      className={`px-4 py-2.5 rounded-full text-[13px] font-semibold border transition-all ${formData.isExServiceman === (val === 'Yes') ? 'bg-secondary text-white border-secondary shadow-sm' : 'bg-white text-text-muted border-gray-200 hover:border-gray-300'}`}>{val}</button>
                  ))}
                </div>
              </div>
            </div>
          )}
        </div>
        <div className="mt-8 flex flex-col items-end gap-3">
          {(!formData.beneficiaryType || !isValidAge || !formData.gender || !formData.category) && (
            <div className="text-[13px] text-red-500 font-medium bg-red-50 px-4 py-2 rounded-lg border border-red-100 flex items-center gap-2">
              <AlertCircle className="w-4 h-4" />Please fill all required fields before continuing.
            </div>
          )}
          <button disabled={!formData.beneficiaryType || !isValidAge || !formData.gender || !formData.category} onClick={() => handleNext('financial')}
            className={`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center justify-center gap-2 ${formData.beneficiaryType && isValidAge && formData.gender && formData.category ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}`}>
            Continue <ArrowRight className="w-5 h-5" />
          </button>
        </div>
      </div>
    );
  };

  const renderFinancial = () => (
    <div className="max-w-2xl mx-auto mt-8 mb-24 px-4">
      <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-8 text-center">Financial Information</h2>
      <div className="bg-white rounded-[20px] p-6 md:p-8 border border-gray-100 shadow-sm flex flex-col gap-6">
        <div>
          <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Annual Family Income (₹)</label>
          <div className="relative">
            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-[16px] font-bold text-text-muted">₹</span>
            <input type="text" placeholder="e.g. 300000"
              value={formData.income ? new Intl.NumberFormat('en-IN').format(Number(String(formData.income).replace(/\D/g, ''))) : ''}
              onChange={(e) => { const rawValue = e.target.value.replace(/\D/g, ''); setFormData({ ...formData, income: rawValue }); }}
              className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 pl-8 pr-4 text-[16px] font-bold text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary" />
          </div>
          <p className="text-[12px] text-text-muted mt-1.5 flex items-center gap-1"><Info className="w-3.5 h-3.5" /> Total household income per year</p>
        </div>
      </div>
      <div className="mt-10 flex justify-end items-center">
        <button disabled={!formData.income} onClick={() => handleNext('location')}
          className={`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center justify-center gap-2 ${formData.income ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}`}>
          Continue <ArrowRight className="w-5 h-5" />
        </button>
      </div>
    </div>
  );

  const renderLocation = () => (
    <div className="max-w-2xl mx-auto mt-8 mb-24 px-4">
      <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-8 text-center">Where are you located?</h2>
      <div className="bg-white rounded-[20px] p-6 md:p-8 border border-gray-100 shadow-sm flex flex-col gap-6">
        <div>
          <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">State / UT</label>
          <select value={formData.state} onChange={handleStateChange} className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3 px-4 text-[15px] text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary appearance-none">
            <option value="">Select state</option>
            {STATES.map(s => <option key={s.code} value={s.code}>{s.name}</option>)}
          </select>
        </div>
        <div>
          <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">District</label>
          <select value={formData.district} onChange={handleDistrictChange} disabled={!formData.state || isLoadingDistricts}
            className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3 px-4 text-[15px] text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary appearance-none disabled:opacity-50 disabled:cursor-not-allowed">
            <option value="" disabled>{isLoadingDistricts ? 'Loading districts...' : 'Select district'}</option>
            {availableDistricts.map(district => <option key={district.code} value={district.code}>{district.name}</option>)}
          </select>
        </div>
      </div>
      <div className="mt-10 flex justify-end items-center">
        <button disabled={!formData.state || !formData.district || isFetchingQuestions}
          onClick={async () => {
            setIsFetchingQuestions(true);
            const questions = await fetchDynamicQuestions(formData);
            setIsFetchingQuestions(false);
            if (questions.length > 0) { setDynamicQuestions(questions); handleNext('specifics'); }
            else { handleNext('review'); }
          }}
          className={`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center justify-center gap-2 ${formData.state && formData.district && !isFetchingQuestions ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}`}>
          {isFetchingQuestions ? 'Loading...' : <>Continue <ArrowRight className="w-5 h-5" /></>}
        </button>
      </div>
    </div>
  );

  const renderSpecifics = () => {
    // isPwD can be known from:
    // 1. formData.isPwD (boolean) — set on the About step
    // 2. formData.dynamicAnswers.IsPwD (string "true"/"false") — answered as a dynamic question on this step
    const isPwDNo = formData.isPwD === false || formData.dynamicAnswers?.IsPwD === 'false';

    // Defensive frontend filter: never render DisabilityPercentage when PwD is No from either source
    const activeQuestions = dynamicQuestions.filter(q => {
      if (q.id === 'DisabilityPercentage' && isPwDNo) return false;
      return true;
    });

    // isComplete is computed ONLY over currently visible active questions
    const isComplete = activeQuestions.every(q => {
      const val = formData.dynamicAnswers[q.id];
      if (q.type === 'taxonomy') return val && val.activity && (val.activity !== 'Other' || val.customActivityText);
      if (q.id === 'DisabilityPercentage') {
        const num = Number(val);
        return !!val && !isNaN(num) && num >= 0 && num <= 100;
      }
      return !!val;
    });

    return (
      <div className="max-w-2xl mx-auto mt-8 mb-24 px-4">
        <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-8 text-center">Additional Details</h2>
        <div className="bg-white rounded-[20px] p-6 md:p-8 border border-gray-100 shadow-sm flex flex-col gap-6">
          {activeQuestions.length === 0 ? (
            <div className="text-center py-8"><p className="text-[15px] text-text-muted">No specific details required for this purpose yet.</p></div>
          ) : (
            activeQuestions.map(q => (
              <div key={q.id} className="relative z-20">
                <label className="block text-[15px] font-bold text-primary mb-3">{q.label}</label>
                {q.type === 'single_choice' && (
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                    {q.options?.map(opt => (
                      <button key={opt.value}
                        onClick={() => {
                          if (q.id === 'IsPwD') {
                            // Sync dynamic IsPwD answer → top-level formData.isPwD boolean
                            // and clear stale DisabilityPercentage if selecting No
                            const isNo = opt.value === 'false';
                            const newDynamic = { ...formData.dynamicAnswers, IsPwD: opt.value };
                            if (isNo) delete newDynamic.DisabilityPercentage;
                            setFormData({ ...formData, isPwD: !isNo, dynamicAnswers: newDynamic });
                          } else {
                            setFormData({ ...formData, dynamicAnswers: { ...formData.dynamicAnswers, [q.id]: opt.value } });
                          }
                        }}
                        className={`py-3 px-4 rounded-xl font-bold text-[14px] border-2 transition-all text-left ${formData.dynamicAnswers[q.id] === opt.value ? 'border-secondary bg-soft-teal text-secondary' : 'border-gray-100 bg-white text-text-muted hover:border-gray-200'}`}>
                        {opt.label}
                      </button>
                    ))}
                  </div>
                )}
                {q.type === 'yes_no' && (
                  <div className="flex gap-3">
                    {['Yes', 'No'].map(opt => (
                      <button key={opt} onClick={() => setFormData({ ...formData, dynamicAnswers: { ...formData.dynamicAnswers, [q.id]: opt } })}
                        className={`flex-1 py-3 px-6 rounded-xl font-bold text-[14px] border-2 transition-all text-center ${formData.dynamicAnswers[q.id] === opt ? 'border-secondary bg-soft-teal text-secondary' : 'border-gray-100 bg-white text-text-muted hover:border-gray-200'}`}>{opt}</button>
                    ))}
                  </div>
                )}
                {q.type === 'text' && (
                  <input type="text" value={formData.dynamicAnswers[q.id] || ''}
                    onChange={(e) => setFormData({ ...formData, dynamicAnswers: { ...formData.dynamicAnswers, [q.id]: e.target.value } })}
                    className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 px-4 text-[15px] text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary" />
                )}
                {q.type === 'numeric' && (
                  <input type="number"
                    min={q.id === 'DisabilityPercentage' ? 0 : undefined}
                    max={q.id === 'DisabilityPercentage' ? 100 : undefined}
                    value={formData.dynamicAnswers[q.id] || ''}
                    onChange={(e) => setFormData({ ...formData, dynamicAnswers: { ...formData.dynamicAnswers, [q.id]: e.target.value } })}
                    className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 px-4 text-[15px] text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary" />
                )}
                {q.type === 'currency' && (
                  <div className="relative">
                    <span className="absolute left-4 top-1/2 -translate-y-1/2 text-[16px] font-bold text-text-muted">₹</span>
                    <input type="text" placeholder="e.g. 50000"
                      value={formData.dynamicAnswers[q.id] ? new Intl.NumberFormat('en-IN').format(Number(formData.dynamicAnswers[q.id])) : ''}
                      onChange={(e) => { const rawValue = e.target.value.replace(/\D/g, ''); setFormData({ ...formData, dynamicAnswers: { ...formData.dynamicAnswers, [q.id]: rawValue } }); }}
                      className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 pl-8 pr-4 text-[16px] font-bold text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary" />
                  </div>
                )}
                {q.type === 'taxonomy' && (
                  <TaxonomySelector value={formData.dynamicAnswers[q.id] || { activity: '', customActivityText: '' }}
                    onChange={(val) => setFormData({ ...formData, dynamicAnswers: { ...formData.dynamicAnswers, [q.id]: val } })} />
                )}
                {q.helpText && (
                  <div className="mt-2 text-[12px] text-text-muted flex items-start gap-1.5 leading-relaxed">
                    <Info className="w-4 h-4 flex-shrink-0 mt-0.5 text-blue-400" /><p>{q.helpText}</p>
                  </div>
                )}
              </div>
            ))
          )}
        </div>
        <div className="mt-10 flex justify-end items-center">
          <button disabled={!isComplete} onClick={() => handleNext('review')}
            className={`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center justify-center gap-2 ${isComplete ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}`}>
            Continue <ArrowRight className="w-5 h-5" />
          </button>
        </div>
      </div>
    );
  };

  const renderReview = () => (
    <div className="max-w-2xl mx-auto mt-8 mb-24 px-4">
      <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-8 text-center">Review Your Profile</h2>
      <div className="bg-white rounded-[20px] border border-gray-100 shadow-sm overflow-hidden">
        <div className="px-6 py-5 border-b border-gray-50 flex items-center justify-between">
          <div className="text-[14px] font-bold text-primary">Purpose</div>
          <div className="text-[15px] font-semibold text-primary">{PURPOSES.find(p => p.id === formData.purpose)?.title || formData.purpose}</div>
        </div>
        <div className="px-6 py-5 border-b border-gray-50 flex items-center justify-between">
          <div className="text-[14px] font-bold text-primary">Profile</div>
          <div className="text-right">
            <div className="text-[15px] font-semibold text-primary">{formData.gender} · {formData.category}</div>
            {formData.dob && <div className="text-[12px] text-text-muted mt-0.5">DoB: {formData.dob}</div>}
          </div>
        </div>
        <div className="px-6 py-5 border-b border-gray-50 flex items-center justify-between">
          <div className="text-[14px] font-bold text-primary">Income</div>
          <div className="text-[15px] font-semibold text-primary">₹{formData.income ? new Intl.NumberFormat('en-IN').format(Number(formData.income)) : 'N/A'}</div>
        </div>
        <div className="px-6 py-5 border-b border-gray-50 flex items-center justify-between">
          <div className="text-[14px] font-bold text-primary">Location</div>
          <div className="text-right"><div className="text-[15px] font-semibold text-primary">{formData.districtName}, {formData.stateName}</div></div>
        </div>
        {dynamicQuestions.length > 0 && (
          <div className="px-6 py-5 border-b border-gray-50">
            <div className="flex items-center justify-between mb-3">
              <div className="text-[14px] font-bold text-primary">Additional Details</div>
              <button onClick={() => handleNext('specifics')} className="text-[13px] font-semibold text-secondary">Edit</button>
            </div>
            {dynamicQuestions.map(q => {
              let displayValue = formData.dynamicAnswers[q.id];
              if (q.type === 'taxonomy' && displayValue) displayValue = displayValue.activity === 'Other' ? displayValue.customActivityText : ACTIVITY_CATEGORIES.find(c => c.id === displayValue.activity)?.title || displayValue.activity;
              else if (q.type === 'currency' && displayValue) displayValue = `₹${new Intl.NumberFormat('en-IN').format(Number(displayValue))}`;
              return (<div key={q.id} className="mb-2"><div className="text-[12px] font-bold text-text-muted uppercase tracking-wide mb-1">{q.label}</div><div className="text-[15px] font-semibold text-primary">{displayValue || 'N/A'}</div></div>);
            })}
          </div>
        )}
      </div>
      <div className="mt-8 text-center">
        <p className="text-[13px] text-text-muted mb-6">ArthSetu will check the latest verified scheme information available from connected Government sources.</p>
        <button onClick={async () => {
          handleNext('processing');
          const sanitizedAnswers: Record<string, string> = {};
          for (const [key, val] of Object.entries(formData.dynamicAnswers || {})) {
            if (typeof val === 'string') sanitizedAnswers[key] = val.replace(/[^0-9.]/g, '');
          }
          const payload = { ...formData, income: formData.income ? String(formData.income).replace(/[^0-9.]/g, '') : formData.income, dynamicAnswers: sanitizedAnswers };
          try {
            const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
            const res = await fetch(`${baseUrl}/api/schemes/match`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
            const data = await res.json();
            setSchemeResults(data);
            handleNext('result');
          } catch (err) {
            console.error(err);
            setSchemeResults({ recommended: [], otherEligible: [], moreInfoNeeded: [], notEligible: [] });
            handleNext('result');
          }
        }} className="bg-secondary text-white px-10 py-4 rounded-[12px] font-bold text-[16px] hover:bg-secondary/90 hover:-translate-y-0.5 hover:shadow-lg transition-all flex items-center justify-center gap-2 mx-auto">
          Find My Schemes <Search className="w-5 h-5" />
        </button>
      </div>
    </div>
  );

  const renderProcessing = () => (
    <div className="max-w-2xl mx-auto text-center mt-24 mb-24 px-4">
      <div className="flex flex-col items-center gap-6">
        <div className="w-16 h-16 rounded-full border-4 border-secondary border-t-transparent animate-spin" />
        <div><h2 className="text-2xl font-extrabold text-primary mb-2">Checking Schemes</h2><p className="text-text-muted text-[15px]">Matching your profile against verified Government schemes…</p></div>
      </div>
    </div>
  );

  const renderResult = () => {
    if (!schemeResults) return null;

    const allSchemes = schemeResults.recommended || [];
    const filteredSchemes = currentFilter === 'All' 
        ? allSchemes 
        : currentFilter === 'Applications Open'
            ? allSchemes.filter((s: any) => s.applicationRoute?.isOpen)
            : allSchemes.filter((s: any) => s.benefitType === currentFilter || s.schemeCategory === currentFilter);

    const totalPages = Math.ceil(filteredSchemes.length / ITEMS_PER_PAGE);
    const paginatedSchemes = filteredSchemes.slice((currentPage - 1) * ITEMS_PER_PAGE, currentPage * ITEMS_PER_PAGE);

    const getAccentColor = (type: string) => {
        if (!type) return 'bg-gray-100 text-gray-700';
        type = type.toLowerCase();
        if (type.includes('loan') || type.includes('credit')) return 'bg-blue-50 text-blue-700 border-blue-200';
        if (type.includes('scholarship') || type.includes('education')) return 'bg-purple-50 text-purple-700 border-purple-200';
        if (type.includes('subsidy') || type.includes('grant') || type.includes('agriculture')) return 'bg-green-50 text-green-700 border-green-200';
        if (type.includes('skill') || type.includes('employment')) return 'bg-amber-50 text-amber-700 border-amber-200';
        return 'bg-teal-50 text-teal-700 border-teal-200';
    };

    const getAppButton = (scheme: any) => {
        const route = scheme.applicationRoute;
        if (!route) return <button className="w-full py-3 bg-gray-100 text-gray-500 font-bold rounded-lg cursor-not-allowed">Application Status Not Verified</button>;
        if (route.isOpen === false) return <button className="w-full py-3 bg-gray-100 text-gray-500 font-bold rounded-lg cursor-not-allowed">Applications Closed</button>;
        if (route.isOpen === null) return <button className="w-full py-3 bg-gray-100 text-gray-500 font-bold rounded-lg cursor-not-allowed">Not Yet Open</button>;
        
        const mode = route.mode;
        let ctaText = 'Apply on Official Portal';
        if (scheme.name.includes('JanSamarth')) ctaText = 'Apply on JanSamarth';
        else if (scheme.name.includes('Scholarship')) ctaText = 'Apply on National Scholarship Portal';
        else if (mode === 'PARTNER_ROUTED') ctaText = 'Find Authorized Partner';
        else if (mode === 'INSTITUTION_ROUTED') ctaText = 'Apply Through Institution';
        else if (mode === 'CSC_ROUTED') ctaText = 'Apply Through CSC';
        else if (mode === 'OFFLINE') ctaText = 'View Application Process';

        return <a href={route.url || '#'} target="_blank" rel="noreferrer" className="w-full block text-center py-3 bg-teal-600 hover:bg-teal-700 text-white font-bold rounded-lg transition-colors">{ctaText}</a>;
    };

    return (
      <div className="max-w-4xl mx-auto mt-4 mb-24 px-4">
        <button onClick={() => { setStep('review'); setStepHistory(prev => prev.slice(0, -1)); }} className="flex items-center gap-1.5 text-text-muted font-bold hover:text-primary transition-colors mb-6 text-[14px]"><ArrowLeft className="w-4 h-4" />Back to Review</button>
        
        <div className="mb-10 text-center">
          <h2 className="text-3xl md:text-4xl font-extrabold text-[#0B1B3D] mb-4 tracking-tight">Your Scheme Results</h2>
          <p className="text-[16px] text-gray-600 mb-6">Verified Government schemes matched to your profile</p>
          
          <div className="flex flex-wrap justify-center gap-3">
             <div className="px-4 py-2 bg-green-50 text-green-700 font-bold rounded-full text-[13px] border border-green-200 flex items-center gap-2"><ShieldCheck className="w-4 h-4" /> {allSchemes.length} Eligible</div>
             {schemeResults.moreInfoNeeded?.length > 0 && <div className="px-4 py-2 bg-amber-50 text-amber-700 font-bold rounded-full text-[13px] border border-amber-200 flex items-center gap-2"><AlertCircle className="w-4 h-4" /> {schemeResults.moreInfoNeeded.length} More Info Needed</div>}
          </div>
        </div>

        {allSchemes.length === 0 ? (
            <div className="bg-white rounded-2xl p-10 text-center shadow-sm border border-gray-100">
                <div className="w-16 h-16 bg-gray-50 rounded-full flex items-center justify-center mx-auto mb-4">
                    <Search className="w-8 h-8 text-gray-400" />
                </div>
                <h3 className="text-xl font-bold text-[#0B1B3D] mb-2">No Verified Schemes Found</h3>
                <p className="text-gray-600 mb-6">No verified Government scheme matched this profile with the information currently available.</p>
                <div className="flex justify-center gap-4">
                    <button onClick={() => setStep('intro')} className="px-6 py-3 bg-teal-600 text-white font-bold rounded-lg hover:bg-teal-700">Edit Profile</button>
                    <button onClick={() => setStep('review')} className="px-6 py-3 bg-white border-2 border-gray-200 text-gray-700 font-bold rounded-lg hover:bg-gray-50">Back to Review</button>
                </div>
            </div>
        ) : (
            <>
                <div className="flex gap-2 overflow-x-auto pb-4 mb-6 scrollbar-hide">
                    {['All', 'Applications Open', 'Loan', 'Scholarship', 'Subsidy', 'Central', 'State'].map(f => (
                        <button key={f} onClick={() => {setCurrentFilter(f); setCurrentPage(1);}} className={`px-4 py-2 rounded-full text-[13px] font-bold whitespace-nowrap transition-colors ${currentFilter === f ? 'bg-[#0B1B3D] text-white' : 'bg-white text-gray-600 hover:bg-gray-50 border border-gray-200'}`}>
                            {f}
                        </button>
                    ))}
                </div>

                <div className="space-y-6">
                {paginatedSchemes.map((scheme: any) => (
                    <div key={scheme.id} className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden hover:shadow-md transition-shadow">
                        <div className="p-6 md:p-8">
                            <div className="flex flex-wrap gap-2 mb-4">
                                {scheme.schemeCategory && <span className="px-3 py-1 bg-gray-100 text-gray-700 font-bold text-[11px] uppercase tracking-wider rounded-md border border-gray-200">{scheme.schemeCategory}</span>}
                                {scheme.benefitType && <span className={`px-3 py-1 font-bold text-[11px] uppercase tracking-wider rounded-md border ${getAccentColor(scheme.benefitType)}`}>{scheme.benefitType}</span>}
                            </div>
                            
                            <h3 className="text-2xl font-extrabold text-[#0B1B3D] mb-2 leading-tight">{scheme.name}</h3>
                            <div className="text-[14px] font-bold text-teal-700 mb-4 flex items-center gap-2">
                                <Landmark className="w-4 h-4" /> {scheme.officialSource || 'Government of India'}
                            </div>
                            
                            <p className="text-gray-600 text-[15px] leading-relaxed mb-6">{scheme.description}</p>
                            
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6 bg-gray-50/50 rounded-xl p-5 mb-6 border border-gray-50">
                                <div>
                                    <div className="text-[11px] font-bold text-gray-500 uppercase tracking-wider mb-1">Application Status</div>
                                    <div className="font-bold text-[#0B1B3D]">{scheme.applicationRoute?.isOpen === true ? 'OPEN' : scheme.applicationRoute?.isOpen === false ? 'CLOSED' : scheme.applicationRoute?.isOpen === null ? 'NOT YET OPEN' : 'UNKNOWN'}</div>
                                </div>
                                <div>
                                    <div className="text-[11px] font-bold text-gray-500 uppercase tracking-wider mb-1">Last Verified</div>
                                    <div className="font-bold text-[#0B1B3D]">{scheme.lastVerified || 'Date not available'}</div>
                                </div>
                            </div>

                            <div className="flex flex-col sm:flex-row gap-4">
                                <div className="flex-1">
                                    {getAppButton(scheme)}
                                </div>
                                <button onClick={() => toggleExpanded(scheme.id, 'details')} className="px-6 py-3 bg-white border-2 border-gray-200 text-gray-700 font-bold rounded-lg hover:bg-gray-50 flex items-center justify-center gap-2">
                                    Why this matched <ChevronDown className={`w-4 h-4 transition-transform ${expandedDetails[scheme.id] ? 'rotate-180' : ''}`} />
                                </button>
                            </div>
                        </div>
                        
                        {expandedDetails[scheme.id] && (
                            <div className="bg-gray-50 border-t border-gray-100 p-6 md:p-8">
                                <h4 className="font-bold text-[#0B1B3D] mb-4">Eligibility Criteria Match</h4>
                                <div className="space-y-3">
                                    {scheme.ruleComparisons?.filter((r: any) => r.status === 'Matched').map((evalItem: any, idx: number) => (
                                        <div key={idx} className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 p-4 bg-white rounded-lg border border-gray-100">
                                            <div>
                                                <div className="text-[14px] font-bold text-[#0B1B3D] mb-1">{evalItem.ruleName}</div>
                                                <div className="text-[12px] text-gray-500"><span className="font-semibold text-gray-700">Scheme Requirement:</span> {evalItem.schemeCondition}</div>
                                                <div className="text-[12px] text-gray-500"><span className="font-semibold text-gray-700">Your Profile:</span> {evalItem.userValue}</div>
                                            </div>
                                            <div className="px-3 py-1 bg-green-50 text-green-700 border border-green-200 rounded-full text-[11px] font-bold uppercase tracking-wider self-start sm:self-center">
                                                Matched
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>
                ))}
                </div>

                {totalPages > 1 && (
                    <div className="flex items-center justify-center gap-2 mt-8">
                        <button disabled={currentPage === 1} onClick={() => setCurrentPage(p => p - 1)} className="p-2 rounded-lg border border-gray-200 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed text-[#0B1B3D]"><ArrowLeft className="w-5 h-5" /></button>
                        <span className="text-[14px] font-bold text-gray-600 mx-4">Page {currentPage} of {totalPages}</span>
                        <button disabled={currentPage === totalPages} onClick={() => setCurrentPage(p => p + 1)} className="p-2 rounded-lg border border-gray-200 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed text-[#0B1B3D]"><ArrowRight className="w-5 h-5" /></button>
                    </div>
                )}
            </>
        )}
      </div>
    );
  };

  return (
    <div className="min-h-[calc(100vh-160px)] bg-bg-main w-full py-8 md:py-12">
      {step !== 'intro' && step !== 'processing' && step !== 'result' && (
        <div className="max-w-2xl mx-auto px-4 mb-12">
          <button onClick={handleBack} className="flex items-center gap-1.5 text-text-muted font-bold hover:text-primary transition-colors mb-4 text-[14px]"><ArrowLeft className="w-4 h-4" />Back</button>
          <div className="flex items-center justify-between mb-2">
            <span className="text-[12px] font-bold text-text-muted uppercase tracking-wider">Stage {getStepNumber()}</span>
            <span className="text-[12px] font-bold text-primary">
              {step === 'purpose' && 'Purpose'}{step === 'about' && 'About You'}{step === 'financial' && 'Financial'}
              {step === 'specifics' && 'Specifics'}{step === 'location' && 'Location'}{step === 'review' && 'Review'}
            </span>
          </div>
          <div className="w-full h-1.5 bg-gray-100 rounded-full overflow-hidden">
            <div className="h-full bg-secondary transition-all duration-500 ease-out rounded-full" style={{ width: `${Math.min(100, (getStepNumber() / 6) * 100)}%` }}></div>
          </div>
        </div>
      )}
      {step === 'intro' && renderIntro()}
      {step === 'purpose' && renderPurpose()}
      {step === 'about' && renderAbout()}
      {step === 'financial' && renderFinancial()}
      {step === 'specifics' && renderSpecifics()}
      {step === 'location' && renderLocation()}
      {step === 'review' && renderReview()}
      {step === 'processing' && renderProcessing()}
      {step === 'result' && renderResult()}
    </div>
  );
}
