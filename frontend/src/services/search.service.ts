import { IRankingResult } from "../types/RankingResult";
import { ISearchRequest } from "../types/SearchRequest";
import { ISupportBrowser } from "../types/SupportBrowser";
import APIService from "./base";

export default class SearchService {
  static async search(request: ISearchRequest): Promise<IRankingResult[]> {
    return APIService.get(
      `/search?keyword=${request.keyword}&url=${request.url}&browserType=${request.browserType}`,
      request
    );
  }

  static async getSupportBrowsers(): Promise<ISupportBrowser[]> {
    return APIService.get("/get-support-browsers");
  }
}
