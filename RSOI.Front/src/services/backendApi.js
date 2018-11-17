import FilesApi from "./api/filesApi";
import axios from "axios";
import JobsApi from "./api/jobsApi";

export default class BackendApiService{
    _apiServices;
    _axios;

    constructor() {
        this._axios = axios.create({
            baseURL: "http://localhost:5000"
        });
        this._apiServices = {
            Files: new FilesApi(this._axios),
            Jobs: new JobsApi(this._axios)
        }
    }

    get API(){
        return this._apiServices;
    }
}