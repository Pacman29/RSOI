import {observable, action} from "mobx";
import Job from "../models/job";


export default class JobsStore {
    @observable isLoading;

    constructor(apiService){
        this.isLoading = false;
        this._apiService = apiService
    }

    @action getAllJobs(){
        this.isLoading = true;
        return this._apiService.API.Jobs.allJobs()
            .then(action((res) => {
            this.isLoading = false;
            return res.map(jsonJob => Job.fromJson(jsonJob));
        })).catch(action(e => {
            this.isLoading = false;
            return e;
        })).finally(action(() => {
            this.isLoading = false;
        }));
    }
}