import BaseApi from "./baseApi";

export default class JobsApi extends BaseApi{
    constructor(axios) {
        super(axios);
    }

    async recognizePdf(pdf){
        try {
            let bodyFormData = new FormData();
            bodyFormData.set('file',pdf);
            let result = await this.axios.post(`api/job`,bodyFormData,{ headers: { 'Content-Type': 'multipart/form-data'}});
            console.log(result);
            return result.data
        } catch (e) {
            throw e;
        }
    }

    async updateJob(jobId,pdf){
        try {
            let bodyFormData = new FormData();
            bodyFormData.set('file',pdf);
            let result = await this.axios.patch(`api/job/${jobId}`,bodyFormData,{ headers: { 'Content-Type': 'multipart/form-data'}});
            console.log(result);
            return result.data
        } catch (e) {
            throw e;
        }
    }

    async deleteJob(jobId){
        try {
            let result = await this.axios.delete(`api/job/${jobId}`);
            console.log(result);
            return result.data
        } catch (e) {
            throw e;
        }
    }

    async allJobs(){
        try {
            let result = await this.axios.get(`/api/job`);
            console.log(result);
            return result.data
        } catch (e) {
            throw e;
        }
    }

    async getJobInfo(jobId){
        try {
            let result = await this.axios.get(`api/job/${jobId}`);
            console.log(result);
            return result.data
        } catch (e) {
            throw e;
        }
    }
}