import BaseApi from "./baseApi";

export default class FilesApi extends BaseApi{
    constructor(axios) {
        super(axios);
    }

    async getImage(jobId,pageNo = 0){
        try {
            let result = await this.axios.get(`api/files/${jobId}/image?PageNo=${pageNo}`);
            console.log(result);
            return result.data
        } catch (e) {
            throw e;
        }
    }

    async getPdf(jobId){
        try {
            let result = await this.axios.get(`api/files/${jobId}/pdf`);
            console.log(result);
            return result.data
        } catch (e) {
            throw e;
        }
    }

    async getImages(jobId,firstPage = 0,count = 0){
        try {
            let result = await this.axios.get(`api/files/${jobId}/images?FirstPage=${firstPage}&Count=${count}`);
            console.log(result);
            return result.data
        } catch (e) {
            throw e;
        }
    }
}