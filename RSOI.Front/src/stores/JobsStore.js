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

    getStoreForJobId(jobId){
        return new JobsStoreByJobId(this._apiService,jobId);
    }
}

class JobsStoreByJobId{
    @observable isLoading;

    constructor(apiService, jobId){
        this.isLoading = false;
        this._apiService = apiService;
        this._jobId = jobId;
    }

    @action getJobInfo(){
        this.isLoading = true;
        return this._apiService.API.Jobs.getJobInfo(this._jobId)
            .then(action((res) => {
                this.isLoading = false;
                return Job.fromJson(res);
            })).catch(action(e => {
                this.isLoading = false;
                return e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }

    @action deleteJob(){
        this.isLoading = true;
        return this._apiService.API.Jobs.deleteJob(this._jobId)
            .then(action((res) => {
                return true;
            })).catch(action(e => {
                this.isLoading = false;
                return e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }
}