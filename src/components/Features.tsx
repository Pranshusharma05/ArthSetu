import { Search, Calculator, MapPin, ArrowRight, CheckCircle2, FileText, Globe2, Building2 } from "lucide-react";
import { Link } from 'react-router-dom';

export function Features() {
  return (
    <section className="w-full bg-white py-20 lg:py-28 border-t border-gray-100">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        
        <div className="text-center max-w-3xl mx-auto mb-16">
          <h2 className="text-3xl md:text-[38px] font-extrabold text-primary mb-4 leading-tight">
            Everything You Need to <br className="hidden sm:block" /> Take the Right Next Step
          </h2>
          <p className="text-[17px] text-text-muted">
            No complicated searching. ArthSetu brings the important decisions into one guided flow.
          </p>
        </div>

        {/* 3 Large Features */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {/* Card 1 */}
          <div className="bg-white p-8 rounded-[20px] shadow-[0_4px_20px_rgb(0,0,0,0.03)] border border-gray-100 flex flex-col items-start hover:shadow-[0_8px_30px_rgb(18,59,109,0.08)] transition-all duration-300">
            <div className="relative mb-6">
               <div className="w-14 h-14 bg-soft-blue rounded-[14px] flex items-center justify-center">
                 <Search className="w-6 h-6 text-primary absolute -ml-2 -mt-2" />
                 <FileText className="w-6 h-6 text-primary opacity-60 absolute ml-3 mt-3" />
               </div>
            </div>
            <h3 className="text-[22px] font-bold text-primary mb-3">Smart Scheme Recommender</h3>
            <p className="text-text-muted text-[15px] leading-relaxed mb-8 flex-grow">
              Tell us your purpose, project cost, income and basic details. ArthSetu identifies suitable schemes and explains why they may fit you.
            </p>
            <Link to="/find-scheme" className="flex items-center gap-2 text-primary font-bold text-[15px] hover:text-secondary transition-colors group mt-auto">
              Find My Scheme 
              <ArrowRight className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
            </Link>
          </div>

          {/* Card 2 */}
          <div className="bg-white p-8 rounded-[20px] shadow-[0_4px_20px_rgb(0,0,0,0.03)] border border-gray-100 flex flex-col items-start hover:shadow-[0_8px_30px_rgb(18,59,109,0.08)] transition-all duration-300">
            <div className="relative mb-6">
               <div className="w-14 h-14 bg-soft-saffron rounded-[14px] flex items-center justify-center">
                 <span className="text-2xl font-bold text-accent absolute -ml-4 -mt-2">₹</span>
                 <Calculator className="w-6 h-6 text-accent absolute ml-2 mt-2" />
               </div>
            </div>
            <h3 className="text-[22px] font-bold text-primary mb-3">Financial & Repayment Planner</h3>
            <p className="text-text-muted text-[15px] leading-relaxed mb-8 flex-grow">
              Understand possible finance, your contribution, interest, tenure, moratorium and repayment before moving ahead.
            </p>
            <button className="flex items-center gap-2 text-primary font-bold text-[15px] hover:text-secondary transition-colors group mt-auto">
              Plan My Finance 
              <ArrowRight className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
            </button>
          </div>

          {/* Card 3 */}
          <div className="bg-white p-8 rounded-[20px] shadow-[0_4px_20px_rgb(0,0,0,0.03)] border border-gray-100 flex flex-col items-start hover:shadow-[0_8px_30px_rgb(18,59,109,0.08)] transition-all duration-300">
            <div className="relative mb-6">
               <div className="w-14 h-14 bg-soft-teal rounded-[14px] flex items-center justify-center">
                 <Building2 className="w-6 h-6 text-secondary absolute -ml-3" />
                 <MapPin className="w-5 h-5 text-primary absolute ml-4 -mt-4 bg-white rounded-full p-0.5" />
               </div>
            </div>
            <h3 className="text-[22px] font-bold text-primary mb-3">Intelligent Partner Locator</h3>
            <p className="text-text-muted text-[15px] leading-relaxed mb-8 flex-grow">
              Find a suitable authorised Channel Partner that supports your selected scheme — not simply the nearest bank.
            </p>
            <button className="flex items-center gap-2 text-primary font-bold text-[15px] hover:text-secondary transition-colors group mt-auto">
              Find a Partner 
              <ArrowRight className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
            </button>
          </div>
        </div>

        {/* Small Secondary Strip */}
        <div className="mt-16 bg-bg-main rounded-2xl p-6 md:p-8 border border-gray-100">
          <div className="text-[14px] font-bold text-primary mb-6 text-center tracking-wide uppercase">Also built into your journey</div>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-6 md:gap-8">
            <div className="flex flex-col items-center text-center gap-2">
              <div className="w-10 h-10 rounded-full bg-white flex items-center justify-center shadow-sm text-secondary mb-1">
                <CheckCircle2 className="w-5 h-5" />
              </div>
              <h4 className="text-[15px] font-bold text-primary">Eligibility Explained</h4>
              <p className="text-[13px] text-text-muted">Know why a rule matched.</p>
            </div>
            <div className="flex flex-col items-center text-center gap-2">
              <div className="w-10 h-10 rounded-full bg-white flex items-center justify-center shadow-sm text-secondary mb-1">
                <FileText className="w-5 h-5" />
              </div>
              <h4 className="text-[15px] font-bold text-primary">Document Readiness</h4>
              <p className="text-[13px] text-text-muted">See what is ready and what is missing.</p>
            </div>
            <div className="flex flex-col items-center text-center gap-2">
              <div className="w-10 h-10 rounded-full bg-white flex items-center justify-center shadow-sm text-secondary mb-1">
                <Globe2 className="w-5 h-5" />
              </div>
              <h4 className="text-[15px] font-bold text-primary">Multilingual Guidance</h4>
              <p className="text-[13px] text-text-muted">Understand schemes in your preferred language.</p>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
