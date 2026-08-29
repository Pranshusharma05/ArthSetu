import React, { useEffect, useState } from 'react';
import { Database, CheckCircle2, AlertCircle } from 'lucide-react';

export default function Admin() {
  const [sources, setSources] = useState<any[]>([]);

  useEffect(() => {
    fetch("`/api/admin/sources`")
      .then(res => res.json())
      .then(data => setSources(data))
      .catch(err => console.error(err));
  }, []);

  return (
    <div className="min-h-screen bg-gray-50 pt-24 pb-12">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-8 flex items-center gap-2">
          <Database className="w-8 h-8 text-blue-600"/> Data Health Dashboard
        </h1>
        <div className="bg-white shadow overflow-hidden sm:rounded-md">
          <ul className="divide-y divide-gray-200">
            {sources.map(s => (
              <li key={s.id} className="p-6 hover:bg-gray-50">
                <div className="flex justify-between items-center">
                  <div>
                    <h3 className="text-lg font-medium text-gray-900">{s.sourceName}</h3>
                    <p className="text-sm text-gray-500">Snapshot: {s.latestSnapshot}</p>
                  </div>
                  <div className="flex items-center gap-6">
                    <div className="text-center">
                      <p className="text-sm font-medium text-gray-500">Verified</p>
                      <p className="text-xl font-bold text-green-600">{s.verifiedCount}</p>
                    </div>
                    <div className="text-center">
                      <p className="text-sm font-medium text-gray-500">Needs Review</p>
                      <p className="text-xl font-bold text-yellow-600">{s.needsReviewCount}</p>
                    </div>
                    <div className="text-center">
                      <p className="text-sm font-medium text-gray-500">Status</p>
                      <p className="text-sm font-bold flex items-center gap-1 ">
                        {s.connectionStatus === 'Connected' ? <CheckCircle2 size={16}/> : <AlertCircle size={16}/>}
                        {s.connectionStatus}
                      </p>
                    </div>
                  </div>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </div>
    </div>
  );
}

