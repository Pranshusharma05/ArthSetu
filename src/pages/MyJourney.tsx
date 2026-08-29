import React from 'react';
import { Bookmark, FileCheck } from 'lucide-react';

export default function MyJourney() {
  return (
    <div className="min-h-screen bg-gray-50 pt-24 pb-12">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-8 flex items-center gap-2">
          <Bookmark className="w-8 h-8 text-blue-600"/> My Journey
        </h1>
        <div className="bg-white shadow rounded-xl p-8 text-center border border-gray-100">
          <FileCheck className="w-16 h-16 text-gray-300 mx-auto mb-4"/>
          <h2 className="text-xl font-bold text-gray-700">No saved applications yet</h2>
          <p className="text-gray-500 mt-2">Use Find Scheme and Loan Planner to start your journey.</p>
        </div>
      </div>
    </div>
  );
}
