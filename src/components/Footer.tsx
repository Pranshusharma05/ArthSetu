export function Footer() {
  return (
    <footer className="w-full bg-primary text-white pt-20 pb-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        
        <div className="grid grid-cols-1 md:grid-cols-4 gap-12 mb-16">
          
          {/* Brand Col */}
          <div className="col-span-1 md:col-span-1">
            <div className="flex items-center gap-3 mb-6">
              {/* Minimal Bridge Symbol - White */}
              <div className="relative w-10 h-10 bg-white/10 rounded-[14px] flex items-center justify-center overflow-hidden border border-white/20">
                <div className="absolute bottom-2 w-6 h-4 border-t-4 border-l-4 border-r-4 border-white rounded-t-lg"></div>
                <div className="absolute bottom-2 w-full h-[2px] bg-white"></div>
              </div>
              <span className="font-bold text-2xl tracking-wide">ArthSetu</span>
            </div>
            
            <p className="text-white/80 text-[14px] leading-relaxed mb-8 max-w-[260px]">
              Helping beneficiaries understand financial schemes and reach the right authorised financial channel.
            </p>
            
            <div className="flex gap-4 text-[14px] font-bold">
              <a href="#" className="hover:text-accent transition-colors">हिंदी</a>
              <span className="text-white/30">|</span>
              <a href="#" className="text-white hover:text-white/80 transition-colors">English</a>
            </div>
          </div>

          {/* Links Cols */}
          <div className="md:ml-auto">
            <h4 className="font-bold text-[16px] mb-5">Explore</h4>
            <ul className="space-y-3 text-[14px] text-white/70 font-medium">
              <li><a href="#" className="hover:text-white transition-colors">Find Scheme</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Loan Planner</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Find Partner</a></li>
            </ul>
          </div>

          <div className="md:ml-auto">
            <h4 className="font-bold text-[16px] mb-5">Support</h4>
            <ul className="space-y-3 text-[14px] text-white/70 font-medium">
              <li><a href="#" className="hover:text-white transition-colors">Help</a></li>
              <li><a href="#" className="hover:text-white transition-colors">FAQs</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Accessibility</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Contact</a></li>
            </ul>
          </div>

          <div className="md:ml-auto">
            <h4 className="font-bold text-[16px] mb-5">Legal</h4>
            <ul className="space-y-3 text-[14px] text-white/70 font-medium">
              <li><a href="#" className="hover:text-white transition-colors">Privacy</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Terms</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Disclaimer</a></li>
            </ul>
          </div>
        </div>

        {/* Bottom Disclaimer */}
        <div className="border-t border-white/10 pt-8 flex justify-center">
          <p className="text-[12px] text-white/50 text-center leading-relaxed max-w-4xl font-medium">
            Scheme information should be checked against the latest applicable official guidelines. ArthSetu recommendations do not constitute government approval or loan sanction.
          </p>
        </div>
        
      </div>
    </footer>
  );
}
