import { ArrowRight, CheckCircle2, ShieldCheck, FileText, MapPin, Store, Building2, Wallet, ArrowDown, Info } from "lucide-react";
import { Link } from 'react-router-dom';

export function Hero() {
  return (
    <section className="w-full bg-bg-main relative overflow-hidden pt-8 pb-12 md:pt-16 md:pb-20 lg:pt-16 lg:pb-24">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-8 items-start">
          
          {/* Left Content */}
          <div className="lg:col-span-5 flex flex-col items-start gap-6 relative z-10 lg:mt-4">
            <div className="inline-flex items-center gap-2 bg-soft-teal text-secondary px-3 py-1.5 rounded-full font-semibold text-xs tracking-wide">
              <ShieldCheck className="w-3.5 h-3.5" />
              FINANCIAL GUIDANCE FOR SC ENTREPRENEURS
            </div>
            
            <h1 className="text-4xl md:text-5xl lg:text-[56px] font-extrabold text-primary leading-[1.15] tracking-tight">
              Find the <span className="text-secondary">Right Scheme</span><br /> for Your Business
            </h1>
            
            <p className="text-lg text-text-muted leading-relaxed max-w-lg">
              Discover suitable concessional finance schemes, understand your eligibility, plan your repayment and find the right authorised Channel Partner — through one simple guided journey.
            </p>
            
            <div className="flex flex-col sm:flex-row items-center gap-4 w-full sm:w-auto mt-2">
              <Link to="/find-scheme" className="w-full sm:w-auto flex items-center justify-center gap-2 bg-primary text-white px-8 py-3.5 rounded-[12px] font-semibold text-[15px] hover:-translate-y-0.5 hover:shadow-lg transition-all">
                Find My Scheme
                <ArrowRight className="w-5 h-5" />
              </Link>
              <Link to="/find-scheme" className="w-full sm:w-auto flex items-center justify-center bg-white text-primary border border-primary/20 px-8 py-3.5 rounded-[12px] font-semibold text-[15px] hover:-translate-y-0.5 hover:border-primary hover:shadow-sm transition-all shadow-sm">
                Check My Eligibility
              </Link>
            </div>
            
            <div className="flex flex-col sm:flex-row items-start sm:items-center gap-4 sm:gap-6 mt-4 text-[14px] font-medium text-text-muted">
              <div className="flex items-center gap-2">
                <CheckCircle2 className="w-4 h-4 text-secondary" />
                <span>Simple guided process</span>
              </div>
              <div className="flex items-center gap-2">
                <CheckCircle2 className="w-4 h-4 text-secondary" />
                <span>Official-source based information</span>
              </div>
              <div className="flex items-center gap-2">
                <CheckCircle2 className="w-4 h-4 text-secondary" />
                <span>Free to explore</span>
              </div>
            </div>
          </div>
          
          {/* Right Content - Visual Product Story */}
          <div className="lg:col-span-7 relative w-full flex flex-col items-center justify-center mt-12 lg:mt-0">
            
            {/* Visual Container (Positioning Context) */}
            <div className="hero-visual relative w-full max-w-[460px] mx-auto z-10 flex flex-col items-center">
              
              {/* Main Product Card - The Core Story */}
              <div className="main-journey-card relative w-full bg-white rounded-[22px] shadow-[0_12px_40px_rgb(18,59,109,0.06)] border border-gray-100 p-6 lg:p-7 z-10 flex flex-col gap-5">
                
                {/* Header */}
                <div className="text-center pb-3 border-b border-gray-50">
                  <h3 className="text-[18px] font-bold text-primary">Your Financial Journey</h3>
                  <p className="text-[13px] text-text-muted mt-1">Tell us what you need. ArthSetu guides the next steps.</p>
                </div>

                {/* Input Area (Step 1) */}
                <div className="bg-soft-blue/50 rounded-xl p-4">
                  <div className="flex items-center gap-3 mb-4">
                    <div className="w-8 h-8 rounded-full bg-white flex items-center justify-center text-primary shadow-sm shrink-0">
                      <Store className="w-4 h-4" />
                    </div>
                    <div>
                      <div className="text-[11px] font-semibold text-text-muted uppercase tracking-wide">I want support for</div>
                      <div className="text-[15px] font-bold text-primary mt-0.5">Dairy Business</div>
                    </div>
                  </div>
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                    <div className="bg-white px-3 py-2.5 rounded-lg shadow-sm border border-gray-50 relative group">
                      <div className="flex items-center justify-between mb-1">
                        <span className="text-[10px] font-semibold text-text-muted uppercase tracking-wide">Total Project Cost</span>
                        <div className="relative cursor-help">
                          <Info className="w-3.5 h-3.5 text-text-muted/70 hover:text-primary transition-colors" />
                          <div className="absolute hidden group-hover:block bottom-full left-1/2 -translate-x-1/2 mb-2 w-48 bg-primary text-white text-[10px] p-2 rounded shadow-lg z-50 text-center leading-relaxed">
                            Estimated total cost of your proposed dairy project
                          </div>
                        </div>
                      </div>
                      <div className="text-[14px] font-bold text-primary">₹3,00,000</div>
                      <div className="text-[9px] text-text-muted mt-1.5 leading-[1.3]">Estimated total cost of your proposed dairy project</div>
                    </div>
                    <div className="bg-white px-3 py-2.5 rounded-lg shadow-sm border border-gray-50 relative group">
                      <div className="flex items-center justify-between mb-1">
                        <div className="flex items-center gap-1.5">
                          <ShieldCheck className="w-3.5 h-3.5 text-secondary" />
                          <span className="text-[10px] font-semibold text-text-muted uppercase tracking-wide">Annual Family Income</span>
                        </div>
                        <div className="relative cursor-help">
                          <Info className="w-3.5 h-3.5 text-text-muted/70 hover:text-primary transition-colors" />
                          <div className="absolute hidden group-hover:block bottom-full right-0 mb-2 w-48 bg-primary text-white text-[10px] p-2 rounded shadow-lg z-50 text-center leading-relaxed">
                            Family income is used to check whether you meet the scheme's income eligibility criteria. It is not your project investment.
                          </div>
                        </div>
                      </div>
                      <div className="text-[14px] font-bold text-primary">₹3,20,000</div>
                      <div className="text-[9px] text-text-muted mt-1.5 leading-[1.3]">Used to check scheme eligibility</div>
                    </div>
                  </div>
                </div>

                {/* Visual Transition */}
                <div className="relative h-6 flex justify-center items-center">
                  <div className="absolute h-full w-[2px] border-l-2 border-dashed border-secondary/30"></div>
                  <div className="w-6 h-6 bg-white border border-secondary/20 rounded-full flex items-center justify-center z-10 animate-pulse-slow">
                    <ArrowDown className="w-3 h-3 text-secondary" />
                  </div>
                </div>

                {/* Scheme Match Result */}
                <div className="border border-secondary/20 rounded-xl p-4 bg-soft-teal/30 relative">
                  <div className="absolute -top-3 left-4 bg-secondary text-white text-[10px] font-bold px-2 py-0.5 rounded uppercase tracking-wider">Recommended</div>
                  <h4 className="text-[16px] font-bold text-primary mt-1 mb-2.5">Term Loan</h4>
                  <div className="flex flex-wrap gap-1.5">
                    <div className="flex items-center gap-1.5 bg-white border border-secondary/10 px-2 py-1 rounded-md">
                      <CheckCircle2 className="w-3.5 h-3.5 text-secondary" />
                      <span className="text-[10px] font-semibold text-secondary">Income eligibility matched</span>
                    </div>
                    <div className="flex items-center gap-1.5 bg-white border border-secondary/10 px-2 py-1 rounded-md">
                      <CheckCircle2 className="w-3.5 h-3.5 text-secondary" />
                      <span className="text-[10px] font-semibold text-secondary">Business purpose matched</span>
                    </div>
                    <div className="flex items-center gap-1.5 bg-white border border-secondary/10 px-2 py-1 rounded-md">
                      <CheckCircle2 className="w-3.5 h-3.5 text-secondary" />
                      <span className="text-[10px] font-semibold text-secondary">Project size fits</span>
                    </div>
                  </div>
                </div>

                {/* Financial Breakdown */}
                <div className="flex flex-col gap-2 mt-1">
                  <div className="text-[13px] font-bold text-primary border-b border-gray-100 pb-2">How Your Project May Be Financed</div>
                  
                  <div className="flex justify-between items-center text-[12px] mt-1.5">
                     <span className="text-text-muted font-medium">Total Project Cost</span>
                     <span className="font-bold text-primary">₹3,00,000</span>
                  </div>
                  
                  {/* Visual Funding Bar */}
                  <div className="w-full h-2 rounded-full overflow-hidden flex my-1.5">
                     <div className="bg-secondary h-full" style={{width: '90%'}}></div>
                     <div className="bg-accent h-full" style={{width: '10%'}}></div>
                  </div>
                  
                  <div className="flex justify-between items-start mt-0.5">
                     <div className="flex flex-col">
                        <div className="flex items-center gap-1.5">
                           <div className="w-2 h-2 rounded-full bg-secondary"></div>
                           <span className="text-[10px] font-semibold text-primary">Possible Scheme Finance</span>
                        </div>
                        <span className="text-[14px] font-bold text-primary mt-1">Up to ₹2,70,000</span>
                     </div>
                     <div className="flex flex-col items-end">
                        <div className="flex items-center gap-1.5 group relative cursor-help">
                           <span className="text-[10px] font-semibold text-primary">Your Contribution</span>
                           <div className="w-2 h-2 rounded-full bg-accent"></div>
                           <div className="absolute hidden group-hover:block bottom-full right-0 mb-2 w-52 bg-primary text-white text-[10px] p-2 rounded shadow-lg z-50 text-center leading-relaxed">
                            This is the part of the project cost that may need to be arranged outside the scheme-financed amount. The actual contribution may vary according to applicable guidelines.
                           </div>
                        </div>
                        <span className="text-[14px] font-bold text-primary mt-1">From ₹30,000</span>
                     </div>
                  </div>
                  
                  <p className="text-[10px] text-text-muted mt-1.5 leading-relaxed">
                     The scheme may finance up to 90% of the eligible project cost, subject to applicable scheme rules and final assessment.
                  </p>
                </div>

                {/* Repayment Snapshot */}
                <div className="bg-gray-50/50 border border-gray-100 rounded-xl p-4 mt-1">
                   <div className="text-[12px] font-bold text-primary mb-3">Repayment Snapshot</div>
                   <div className="grid grid-cols-2 gap-y-3 gap-x-4 mb-4">
                      <div>
                         <div className="text-[10px] text-text-muted uppercase tracking-wide">Interest Rate</div>
                         <div className="text-[11px] font-semibold text-primary mt-0.5">Scheme-specific rate</div>
                      </div>
                      <div>
                         <div className="text-[10px] text-text-muted uppercase tracking-wide">Repayment Period</div>
                         <div className="text-[11px] font-semibold text-primary mt-0.5">Scheme-specific tenure</div>
                      </div>
                      <div>
                         <div className="text-[10px] text-text-muted uppercase tracking-wide">Moratorium</div>
                         <div className="text-[11px] font-semibold text-primary mt-0.5">Scheme-specific period</div>
                      </div>
                      <div>
                         <div className="text-[10px] text-text-muted uppercase tracking-wide">Repayment Freq.</div>
                         <div className="text-[11px] font-semibold text-primary mt-0.5">Applicable schedule</div>
                      </div>
                   </div>
                   <button className="text-[11px] font-bold text-secondary hover:text-primary transition-colors flex items-center justify-center gap-1 w-full border-t border-gray-100 pt-3">
                     View Detailed Repayment Plan <ArrowRight className="w-3.5 h-3.5" />
                   </button>
                </div>

                {/* Bottom Actions */}
                <div className="grid grid-cols-2 gap-3 mt-1 relative z-30">
                  <button className="bg-soft-blue text-primary font-semibold text-[13px] py-3 rounded-xl hover:bg-blue-100 transition-colors cursor-pointer">Plan Repayment</button>
                  <button className="bg-primary text-white font-semibold text-[13px] py-3 rounded-xl hover:bg-primary/90 transition-colors cursor-pointer">Find Partner</button>
                </div>

              </div>

              {/* Floating Micro-Cards (Desktop XL Only - guarantees ZERO overlap over main content) */}
              
              {/* Card 1: Eligibility (Upper Left) */}
              <div className="hidden xl:flex floating-card absolute top-[20px] -left-[200px] z-20 bg-white rounded-[14px] shadow-[0_8px_30px_rgb(0,0,0,0.04)] border border-teal-50 p-3.5 items-center gap-3 w-[220px] animate-float-eligibility">
                <div className="w-10 h-10 bg-soft-teal rounded-full flex items-center justify-center shrink-0">
                  <ShieldCheck className="w-5 h-5 text-secondary" />
                </div>
                <div className="flex flex-col">
                  <span className="text-[13px] font-bold text-primary">Eligibility Check</span>
                  <span className="text-[11px] text-text-muted mt-0.5">Basic criteria matched</span>
                  <div className="flex items-center gap-1 mt-1.5">
                    <CheckCircle2 className="w-3.5 h-3.5 text-secondary" />
                    <span className="text-[10px] font-bold text-secondary leading-tight">Eligible for recommendation</span>
                  </div>
                </div>
              </div>

              {/* Card 2: Document Readiness (Upper Right) */}
              <div className="hidden xl:flex floating-card absolute top-[120px] -right-[170px] z-20 bg-white rounded-[14px] shadow-[0_8px_30px_rgb(0,0,0,0.04)] border border-gray-100 p-3.5 flex-col gap-2 w-[190px] animate-float-document">
                <div className="flex items-center gap-2">
                  <FileText className="w-4 h-4 text-primary" />
                  <span className="text-[13px] font-bold text-primary">Document Readiness</span>
                </div>
                <div>
                   <div className="flex justify-between text-[11px] font-semibold text-text-muted mb-1.5">
                     <span>Essentials</span>
                     <span className="text-primary font-bold">80% Ready</span>
                   </div>
                   <div className="w-full h-1.5 bg-gray-100 rounded-full overflow-hidden">
                     <div className="h-full bg-secondary rounded-full animate-progress-fill"></div>
                   </div>
                </div>
              </div>

              {/* Card 3: Authorised Partner (Lower Left) */}
              <div className="hidden xl:flex floating-card absolute bottom-[100px] -left-[210px] z-20 bg-white rounded-[14px] shadow-[0_8px_30px_rgb(0,0,0,0.04)] border border-gray-100 p-3.5 items-start gap-3 w-[230px] animate-float-partner">
                <div className="w-10 h-10 bg-soft-blue rounded-full flex items-center justify-center shrink-0 mt-0.5">
                  <Building2 className="w-5 h-5 text-primary" />
                </div>
                <div className="flex flex-col">
                  <span className="text-[13px] font-bold text-primary">Suitable Partner Found</span>
                  <span className="text-[11px] text-text-muted mt-0.5">Authorised Channel Partner</span>
                  <div className="flex items-center gap-1 mt-1.5">
                    <MapPin className="w-3.5 h-3.5 text-accent" />
                    <span className="text-[11px] font-medium text-text-muted">2.4 km away</span>
                  </div>
                  <div className="inline-flex items-center gap-1 bg-soft-teal px-2 py-0.5 rounded text-[10px] font-bold text-secondary mt-2 w-fit uppercase">
                    <CheckCircle2 className="w-3 h-3" />
                    Scheme supported
                  </div>
                </div>
              </div>

              {/* Card 4: Repayment Explained (Lower Right) */}
              <div className="hidden xl:flex floating-card absolute bottom-[20px] -right-[180px] z-20 bg-white rounded-[14px] shadow-[0_8px_30px_rgb(0,0,0,0.04)] border border-orange-50 p-3.5 items-center gap-3 w-[200px] animate-float-repayment">
                <div className="w-9 h-9 bg-soft-saffron rounded-full flex items-center justify-center shrink-0">
                   <Wallet className="w-4 h-4 text-accent" />
                </div>
                <div className="flex flex-col">
                  <span className="text-[13px] font-bold text-primary">Repayment Ready</span>
                  <span className="text-[11px] text-text-muted mt-0.5">See interest, tenure & schedule</span>
                </div>
              </div>

            </div>
            
            {/* Mobile/Tablet/Laptop Status Cards (Visible below 1280px to guarantee zero overlap) */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 mt-8 xl:hidden w-full max-w-[440px]">
              
              {/* Eligibility Checked */}
              <div className="bg-white rounded-[14px] shadow-sm border border-teal-50 p-3 flex items-center gap-3">
                <div className="w-8 h-8 bg-soft-teal rounded-full flex items-center justify-center shrink-0">
                  <ShieldCheck className="w-4 h-4 text-secondary" />
                </div>
                <div className="flex flex-col">
                  <span className="text-[13px] font-bold text-primary">Eligibility Checked</span>
                  <span className="text-[11px] text-text-muted mt-0.5">Basic criteria matched</span>
                </div>
              </div>
              
              {/* Documents */}
              <div className="bg-white rounded-[14px] shadow-sm border border-gray-100 p-3 flex flex-col justify-center gap-2">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-1.5">
                    <FileText className="w-4 h-4 text-primary" />
                    <span className="text-[13px] font-bold text-primary">Documents</span>
                  </div>
                  <span className="text-[11px] font-bold text-secondary">80% Ready</span>
                </div>
                <div className="w-full h-1.5 bg-gray-100 rounded-full overflow-hidden">
                   <div className="h-full bg-secondary rounded-full animate-progress-fill"></div>
                </div>
              </div>

              {/* Partner Found */}
              <div className="bg-white rounded-[14px] shadow-sm border border-gray-100 p-3 flex items-center gap-3">
                <div className="w-8 h-8 bg-soft-blue rounded-full flex items-center justify-center shrink-0">
                  <Building2 className="w-4 h-4 text-primary" />
                </div>
                <div className="flex flex-col">
                  <span className="text-[13px] font-bold text-primary">Partner Found</span>
                  <div className="flex items-center gap-1 mt-0.5">
                    <MapPin className="w-3 h-3 text-accent" />
                    <span className="text-[11px] font-medium text-text-muted">2.4 km away</span>
                  </div>
                </div>
              </div>

              {/* Repayment */}
              <div className="bg-white rounded-[14px] shadow-sm border border-orange-50 p-3 flex items-center gap-3">
                <div className="w-8 h-8 bg-soft-saffron rounded-full flex items-center justify-center shrink-0">
                   <Wallet className="w-4 h-4 text-accent" />
                </div>
                <div className="flex flex-col">
                  <div className="text-[13px] font-bold text-primary">Repayment Ready</div>
                  <div className="text-[11px] text-text-muted mt-0.5">See interest, tenure & schedule</div>
                </div>
              </div>
            </div>

          </div>

        </div>
      </div>
    </section>
  );
}
