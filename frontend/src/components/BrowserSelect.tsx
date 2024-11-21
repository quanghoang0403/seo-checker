import React, { useEffect, useState } from "react";
import { SearchService } from "../services";
import { ISupportBrowser } from "../types/SupportBrowser";

interface BrowserSelectProps {
  value: number;
  onChange: (browserType: number) => void;
}

const BrowserSelect: React.FC<BrowserSelectProps> = ({ value, onChange }) => {
  const [supportBrowsers, setSupportBrowsers] = useState<ISupportBrowser[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

  useEffect(() => {
    const onFetchSupportBrowsers = async () => {
      try {
        setLoading(true);
        const res = await SearchService.getSupportBrowsers();
        setSupportBrowsers(res);
      } catch (err) {
        alert(err);
      }
      setLoading(false);
    };

    onFetchSupportBrowsers();
  }, []); // Empty dependency array ensures this runs only once

  return (
    <div>
      <label
        htmlFor="browser"
        className="block text-sm font-medium text-gray-700"
      >
        Choose Browser
      </label>
      {!loading && (
        <select
          id="browser"
          value={value}
          onChange={(e) => onChange(Number(e.target.value))}
          className="mt-1 w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring focus:border-blue-300"
          required
        >
          {supportBrowsers.map((browserOption) => (
            <option
              key={browserOption.browserType}
              value={browserOption.browserType}
            >
              {browserOption.browserName}
            </option>
          ))}
        </select>
      )}
    </div>
  );
};

export default BrowserSelect;
