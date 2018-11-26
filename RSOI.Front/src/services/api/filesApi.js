import BaseApi from "./baseApi";

export default class FilesApi extends BaseApi{
    constructor(axios) {
        super(axios);
    }

    getImageURL(jobId,pageNo = 0){
        return `api/files/${jobId}/image?PageNo=${pageNo}`
    }

    getImageURLWithHost(jobId,pageNo = 0){
        return `${this.axios.defaults.baseURL}/${this.getImageURL(jobId,pageNo)}`
    }

    async getImage(jobId,pageNo = 0){
        try {
            let result = await this.axios.get(this.getImageURL(jobId,pageNo),{
                responseType: 'blob',

            });
            console.log(result);
            return new Blob([result.data],{type: "image/jpeg"})
        } catch (e) {
            throw e;
        }
    }

    async getPdf(jobId){
        try {
            let result = await this.axios.get(`api/files/${jobId}/pdf`,{
                responseType: 'blob',
            });
            console.log(result);
            return result.data;
        } catch (e) {
            throw e;
        }
    }

    async getImages(jobId,firstPage = 0,count = 0){
        try {
            let result = await this.axios.get(`api/files/${jobId}/images?FirstPage=${firstPage}&Count=${count}`,{
                responseType: 'blob',
            });
            console.log(result);
            return result.data
        } catch (e) {
            throw e;
        }
    }
}