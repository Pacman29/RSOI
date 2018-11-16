export const enumJobStatus = {
    EXECUTE: 0,
    DONE: 1,
    ERROR: 2,
    REJECTED: 3,

    fromString(str){
        return this[str.toUpperCase()];
    }
};

export default class Job {
    constructor(jobId, status){
        this.jobId = jobId;
        this.status = status;
    }

    static getEnumStatus(str){
        let up = str.toUpperCase();
        return enumJobStatus[up];
    }

    static fromJson(obj){
        return new Job(obj.jobId, enumJobStatus.fromString(obj.jobStatus))
    }
}