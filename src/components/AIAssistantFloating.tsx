import { MessageSquare, X } from "lucide-react";
import { useState } from "react";

export function AIAssistantFloating() {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      {/* Floating Button */}
      <button
        onClick={() => setIsOpen(true)}
        className={`fixed bottom-6 right-6 z-50 bg-primary text-white p-4 rounded-full shadow-lg hover:bg-primary/90 hover:shadow-xl hover:-translate-y-1 transition-all duration-300 flex items-center gap-2 ${isOpen ? 'scale-0 opacity-0' : 'scale-100 opacity-100'}`}
      >
        <MessageSquare className="w-6 h-6" />
        <span className="font-semibold hidden md:inline">Ask ArthSetu</span>
      </button>

      {/* Side Panel / Chat Drawer */}
      <div
        className={`fixed inset-y-0 right-0 z-[100] w-full sm:w-[400px] bg-white shadow-2xl border-l border-gray-100 transform transition-transform duration-300 ease-in-out ${isOpen ? 'translate-x-0' : 'translate-x-full'}`}
      >
        {/* Header */}
        <div className="flex items-center justify-between p-5 border-b border-gray-100 bg-primary text-white">
          <div className="flex items-center gap-3">
            <MessageSquare className="w-5 h-5 text-soft-teal" />
            <div>
              <h3 className="font-bold text-[16px]">ArthSetu Assistant</h3>
              <p className="text-[12px] text-white/70">Hindi • English • Hinglish</p>
            </div>
          </div>
          <button
            onClick={() => setIsOpen(false)}
            className="p-2 hover:bg-white/10 rounded-lg transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Chat Area */}
        <div className="p-5 h-[calc(100vh-140px)] overflow-y-auto flex flex-col gap-4 bg-gray-50/50">
          <div className="bg-white border border-gray-100 p-4 rounded-2xl rounded-tl-sm shadow-sm max-w-[85%] self-start">
            <p className="text-[14px] text-text-main leading-relaxed">
              Namaste! How can I help you today? I can explain schemes, financial terms like moratorium, or help you understand required documents.
            </p>
          </div>
        </div>

        {/* Input Area */}
        <div className="absolute bottom-0 left-0 right-0 p-4 bg-white border-t border-gray-100">
          <div className="relative">
            <input
              type="text"
              placeholder="Ask anything..."
              className="w-full bg-gray-50 border border-gray-200 rounded-full py-3 px-5 pr-12 text-[14px] focus:outline-none focus:border-primary/30 focus:ring-2 focus:ring-primary/10 transition-all"
            />
            <button className="absolute right-2 top-1/2 -translate-y-1/2 p-2 text-primary hover:bg-gray-100 rounded-full transition-colors">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="m22 2-7 20-4-9-9-4Z"/><path d="M22 2 11 13"/></svg>
            </button>
          </div>
        </div>
      </div>

      {/* Backdrop */}
      {isOpen && (
        <div 
          className="fixed inset-0 bg-primary/20 backdrop-blur-sm z-[90] transition-opacity"
          onClick={() => setIsOpen(false)}
        />
      )}
    </>
  );
}
