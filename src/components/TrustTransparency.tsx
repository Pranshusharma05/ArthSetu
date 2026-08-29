import { FileCheck, ShieldAlert, Eye, Lock } from "lucide-react";

export function TrustTransparency() {
  const items = [
    {
      icon: <FileCheck className="w-5 h-5 text-secondary" />,
      title: "Official-Source Based",
      description: "Scheme information is maintained using verified official sources."
    },
    {
      icon: <Eye className="w-5 h-5 text-secondary" />,
      title: "Explainable Eligibility",
      description: "Users can see why a condition matched or did not match."
    },
    {
      icon: <Lock className="w-5 h-5 text-secondary" />,
      title: "Secure & Private",
      description: "Sensitive information is only requested when strictly necessary."
    },
    {
      icon: <ShieldAlert className="w-5 h-5 text-secondary" />,
      title: "No False Approval Claims",
      description: "Recommendations guide you and do not represent guaranteed loan approval."
    }
  ];

  return (
    <section className="w-full bg-soft-blue py-16">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {items.map((item, index) => (
            <div key={index} className="flex flex-col items-center text-center p-4">
              <div className="w-10 h-10 bg-white rounded-full flex items-center justify-center mb-4 shadow-sm">
                {item.icon}
              </div>
              <h4 className="text-[15px] font-bold text-primary mb-1.5">
                {item.title}
              </h4>
              <p className="text-text-muted text-[13px] leading-relaxed">
                {item.description}
              </p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
