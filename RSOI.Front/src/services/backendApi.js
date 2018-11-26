import FilesApi from "./api/filesApi";
import axios from "axios";
import JobsApi from "./api/jobsApi";
import AuthApi from "./api/authApi";

export default class BackendApiService{
    _apiServices;
    _axios;

    constructor() {
        this._axios = axios.create({
            baseURL: "http://localhost:5000"
        });
        this._apiServices = {
            Auth: new AuthApi(this._axios),
            Files: new FilesApi(this._axios),
            Jobs: new JobsApi(this._axios),
        }
    }

    get API(){
        return this._apiServices;
    }
}