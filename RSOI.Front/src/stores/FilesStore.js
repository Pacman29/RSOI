import {action, observable} from "mobx";
import Job from "../models/job";
import FileSaver from "file-saver";

export default class FilesStore {
    @observable isLoading;

    constructor(apiService){
        this.isLoading = false;
        this._apiService = apiService
    }

    @action getImage(jobId, pageNo = 0){
        this.isLoading = true;
        return this._apiService.API.Files.getImage(jobId, pageNo)
            .then(action((res) => {
                this.isLoading = false;
                return res
            })).catch(action(e => {
                this.isLoading = false;
                return e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }

    getFileStoreForJobId(jobId){
        return new JobIdFileStore(this._apiService,jobId);
    }
}

class JobIdFileStore {
    @observable isLoading;

    constructor(apiService, jobId){
        this.isLoading = false;
        this._apiService = apiService;
        this._jobId = jobId;
    }

    @action getImage(pageNo = 0){
        this.isLoading = true;
        return this._apiService.API.Files.getImage(this._jobId, pageNo)
            .then(action((res) => {
                this.isLoading = false;
                return res
            })).catch(action(e => {
                this.isLoading = false;
                return e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }

    @action getPdf(){
        this.isLoading = true;
        return this._apiService.API.Files.getPdf(this._jobId)
            .then(action((res) => {
                FileSaver.saveAs(res,`${this._jobId}.pdf`);
                this.isLoading = false;
                return true
            })).catch(action(e => {
                this.isLoading = false;
                return e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }

    @action getImages(firstPage,count){
        this.isLoading = true;
        return this._apiService.API.Files.getImages(this._jobId,firstPage,count)
            .then(action((res) => {
                FileSaver.saveAs(res,`${this._jobId}.zip`);
                this.isLoading = false;
                return true
            })).catch(action(e => {
                this.isLoading = false;
                return e;
            })).finally(action(() => {
                this.isLoading = false;
            }));
    }
}