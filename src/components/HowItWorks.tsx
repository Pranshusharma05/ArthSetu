import { ArrowRight, UserCheck, ShieldCheck, ListChecks, Building } from "lucide-react";

export function HowItWorks() {
  const steps = [
    {
      number: "01",
      icon: <UserCheck className="w-5 h-5" />,
      title: "Tell Us Your Need",
      description: "Business/education purpose, cost and basic profile."
    },
    {
      number: "02",
      icon: <ShieldCheck className="w-5 h-5" />,
      title: "Get the Right Scheme",
      description: "Eligibility rules are checked and suitable schemes are explained."
    },
    {
      number: "03",
      icon: <ListChecks className="w-5 h-5" />,
      title: "Understand Your Finance",
      description: "See possible finance, contribution and repayment."
    },
    {
      number: "04",
      icon: <Building className="w-5 h-5" />,
      title: "Find the Right Partner",
      description: "Locate a compatible authorised Channel Partner."
    }
  ];

  return (
    <section className="w-full bg-bg-main py-20 lg:py-28">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        
        <div className="text-center max-w-3xl mx-auto mb-16">
          <h2 className="text-3xl md:text-[38px] font-extrabold text-primary mb-4">
            From Your Need to the Right Place to Apply
          </h2>
        </div>

        <div className="flex flex-col lg:flex-row items-start justify-between gap-6 relative mt-12">
          {/* Connector Line (Desktop) */}
          <div className="hidden lg:block absolute top-8 left-12 right-12 h-[2px] bg-gray-200 z-0"></div>

          {steps.map((step, index) => (
            <div key={index} className="relative z-10 flex flex-row lg:flex-col items-center lg:items-start text-left flex-1 group w-full lg:w-auto bg-white lg:bg-transparent p-4 lg:p-0 rounded-[16px] lg:rounded-none shadow-sm lg:shadow-none border border-gray-100 lg:border-none mb-4 lg:mb-0">
              
              {/* Desktop Number/Icon Circle */}
              <div className="w-16 h-16 bg-white border-2 border-gray-200 text-text-muted rounded-full hidden lg:flex items-center justify-center mb-6 shadow-sm group-hover:border-secondary group-hover:text-secondary group-hover:bg-soft-teal transition-all duration-300">
                {step.icon}
              </div>

              {/* Mobile Number */}
              <div className="w-12 h-12 bg-soft-blue text-primary font-bold rounded-[12px] flex lg:hidden items-center justify-center shrink-0 mr-4">
                {step.number}
              </div>

              <div className="flex flex-col">
                <div className="text-[12px] font-bold text-secondary mb-1 hidden lg:block uppercase tracking-wider">Step {step.number}</div>
                <h4 className="text-[18px] font-bold text-primary mb-2">
                  {step.title}
                </h4>
                <p className="text-text-muted text-[14px] leading-relaxed max-w-[240px]">
                  {step.description}
                </p>
              </div>

            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
