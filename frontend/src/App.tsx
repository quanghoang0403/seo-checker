import React, { useState } from "react";
import MainLayout from "./components/MainLayout";
import SearchBar from "./components/SearchBar";
import { IRankingResult } from "./types/RankingResult";
import { SearchService } from "./services";
import { ISearchRequest } from "./types/SearchRequest";
import RankingResult from "./components/RankingResult";

const App: React.FC = () => {
  const [rankingResults, setRankingResults] = useState<IRankingResult[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

  const handleSearch = async (request: ISearchRequest) => {
    try {
      setLoading(true);
      const results = await SearchService.search(request);
      setRankingResults(results);
    } catch (err) {
      alert(err);
    }
    setLoading(false);
  };

  return (
    <MainLayout>
      {loading && (
        <div className="fixed inset-0 bg-gray-800 bg-opacity-70 flex justify-center items-center z-50">
          <div className="w-16 h-16 border-4 border-t-transparent border-blue-500 border-solid rounded-full animate-spin"></div>
        </div>
      )}
      <div className={loading ? "opacity-50 pointer-events-none" : ""}>
        <SearchBar onSubmit={handleSearch} />
        <RankingResult data={rankingResults} />
      </div>
    </MainLayout>
  );
};

export default App;
