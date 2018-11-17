export const enumJobStatus = {
    EXECUTE: "Execute",
    DONE: "Ok",
    ERROR: "Error",
    REJECTED: "Rejected",

    fromString(str){
        return this[str.toUpperCase()];
    }
};

export default class Job {
    constructor(jobId, status, pageCount = 0){
        this.jobId = jobId;
        this.status = status;
        this.pageCount = pageCount
    }

    static getEnumStatus(str){
        let up = str.toUpperCase();
        return enumJobStatus[up];
    }

    static fromJson(obj){
        let length = obj.images === undefined ? 0 : obj.images.length;
        return new Job(obj.jobId, enumJobStatus.fromString(obj.jobStatus),length)
    }
}