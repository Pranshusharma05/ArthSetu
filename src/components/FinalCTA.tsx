import { ArrowRight } from "lucide-react";
import { Link } from 'react-router-dom';

export function FinalCTA() {
  return (
    <section className="w-full bg-gradient-to-r from-soft-blue to-soft-teal py-20 lg:py-24">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center flex flex-col items-center">
        
        <h2 className="text-3xl md:text-[40px] font-extrabold text-primary mb-4 leading-tight">
          Not Sure Which Scheme Is Right for You?
        </h2>
        
        <p className="text-[17px] text-primary/80 mb-10 max-w-2xl">
          Answer a few simple questions and ArthSetu will guide you step by step.
        </p>

        <Link to="/find-scheme" className="flex items-center justify-center gap-2 bg-primary text-white px-10 py-4 rounded-[12px] font-bold text-[16px] hover:bg-opacity-90 hover:-translate-y-0.5 hover:shadow-lg transition-all mb-6 w-full sm:w-auto">
          Start Scheme Check
          <ArrowRight className="w-5 h-5" />
        </Link>

        <Link to="/find-scheme" className="text-primary font-bold text-[15px] hover:text-secondary underline underline-offset-4 transition-colors">
          Explore Schemes
        </Link>
      </div>
    </section>
  );
}
