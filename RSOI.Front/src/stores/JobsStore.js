import {observable, action} from "mobx";
import Job from "../models/job";


export default class JobsStore {
    @observable isLoading;

    constructor(apiService){
        this.isLoading = false;
        this._apiService = apiService
    }

    @action createJob(file){
        this.isLoading = true;
        return this._apiService.API.Jobs.recognizePdf(file)
            .then(action((res) => {
                return Job.fromJson(res);
            })).catch(action(e => {
                this.isLoading = false;
                return e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }

    @action updateJob(jobId,file){
        this.isLoading = true;
        return this._apiService.API.Jobs.updateJob(jobId,file)
            .then(action((res) => {
                return Job.fromJson(res);
            })).catch(action(e => {
                this.isLoading = false;
                return e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }

    @action getAllJobs(){
        this.isLoading = true;
        return this._apiService.API.Jobs.allJobs()
            .then(action((res) => {
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

    @action updateJob(file){
        this.isLoading = true;
        return this._apiService.API.Jobs.updateJob(this._jobId,file)
            .then(action((res) => {
                return Job.fromJson(res);
            })).catch(action(e => {
                this.isLoading = false;
                return e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }
}