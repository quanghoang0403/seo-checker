import React, { useState } from "react";
import BrowserSelect from "./BrowserSelect";
import { ISearchRequest } from "../types/SearchRequest";

interface SearchBarProps {
  onSubmit: (data: ISearchRequest) => void;
}

const SearchBar: React.FC<SearchBarProps> = ({ onSubmit }) => {
  const [keyword, setKeyword] = useState<string>("");
  const [url, setUrl] = useState<string>("");
  const [browserType, setBrowserType] = useState<number>(0);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit({ keyword, url, browserType });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label
          htmlFor="keyword"
          className="block text-sm font-medium text-gray-700"
        >
          Keyword
        </label>
        <input
          type="text"
          id="keyword"
          value={keyword}
          onChange={(e) => setKeyword(e.target.value)}
          placeholder="Enter keyword"
          className="mt-2 w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring focus:border-blue-300"
          required
        />
      </div>
      <div>
        <label
          htmlFor="url"
          className="block text-sm font-medium text-gray-700"
        >
          URL
        </label>
        <input
          type="text"
          id="url"
          value={url}
          onChange={(e) => setUrl(e.target.value)}
          placeholder="Enter URL"
          className="mt-2 w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring focus:border-blue-300"
          required
        />
      </div>

      <BrowserSelect value={browserType} onChange={setBrowserType} />
      <button
        type="submit"
        className="w-full px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
      >
        Search
      </button>
    </form>
  );
};

export default SearchBar;
