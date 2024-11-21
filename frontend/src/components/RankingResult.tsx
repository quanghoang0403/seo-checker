import React from "react";
import { IRankingResult } from "../types/RankingResult";

interface RankingResultProps {
  data: IRankingResult[];
}

const RankingResult: React.FC<RankingResultProps> = ({ data }) => {
  if (data.length === 0) return <></>;
  return (
    <div className="container mx-auto p-4">
      <table className="min-w-full table-auto border-collapse border border-gray-200">
        <thead>
          <tr>
            <th className="py-2 px-4 border-b text-left font-medium text-gray-600">
              Browser Name
            </th>
            <th className="py-2 px-4 border-b text-left font-medium text-gray-600">
              Positions
            </th>
          </tr>
        </thead>
        <tbody>
          {data.map((item, index) => (
            <tr key={index} className="hover:bg-gray-50">
              <td className="py-2 px-4 border-b">{item.browserName}</td>
              <td className="py-2 px-4 border-b">{item.position}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default RankingResult;
