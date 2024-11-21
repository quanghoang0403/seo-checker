import axios, { AxiosResponse } from "axios";
import qs from "qs";

const _configRequest = async (request: any) => {
  if (!request.params) {
    request.params = {};
  }
  request.baseURL = "https://localhost:7239/api/Search";
  request.paramsSerializer = (params: any) => qs.stringify(params);
  return request;
};

const _configResponse = async (response: AxiosResponse<any>) => {
  if (response.data?.code !== 0) {
    console.log(
      "error: ",
      response.data?.errorMessage ?? response.data?.message
    );
    return null;
  }
  return response.data?.data;
};

const _configError = async (error: any) => {
  console.log(error);
  const messageError = error.response.data?.message || error.message;
  if (axios.isCancel(error)) {
    return new Promise((r) => {
      console.log("Cancel:", r);
    });
  }
  if (
    error.request.responseType === "blob" &&
    error.response.data instanceof Blob &&
    error.response.data.type &&
    error.response.data.type.toLowerCase().indexOf("json") !== -1
  ) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();

      reader.onload = () => {
        error.response.data = JSON.parse(reader.result as string);
        resolve(
          Promise.reject({
            ...error,
            message: messageError,
          })
        );
      };

      reader.onerror = () => {
        reject({
          ...error,
          message: messageError,
        });
      };
      reader.readAsText(error?.response?.data);
    });
  }

  return Promise.reject({
    ...error,
    message: messageError,
  });
};

const APIService = axios.create({
  baseURL: process.env.NEXT_PUBLIC_BACKEND,
  headers: {
    "Content-Type": "application/json",
    "Access-Control-Allow-Origin": "*",
  },
});

APIService.interceptors.request.use(_configRequest, _configError);
APIService.interceptors.response.use(_configResponse, _configError);

export default APIService;
