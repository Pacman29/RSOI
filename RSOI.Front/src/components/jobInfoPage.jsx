import React from "react";
import PropTypes from "prop-types";
import {Block, Page, List, ListButton, Stepper} from "framework7-react";
import {inject, observer} from "mobx-react";
import JobsStore from "../stores/JobsStore";
import {computed, observable, action} from "mobx";
import Loader from "./loader";
import Job from "../models/job";
import FilesStore from "../stores/FilesStore";
import AppNavbar from "./appNavbar";

@inject('jobsStore')
@inject('filesStore')
@observer
export default class JobInfoPage extends React.Component {
    static propTypes = {
        jobsStore: PropTypes.instanceOf(JobsStore),
        filesStore: PropTypes.instanceOf(FilesStore),
    };

    @observable _job = new Job("","","");
    firstPage = 0;
    count = 0;

    constructor(props){
        super(props);
        this.jobStore = this.props.jobsStore.getStoreForJobId(this.$f7route.params.jobId);
        this.jobFilesStore = this.props.filesStore.getFileStoreForJobId(this.$f7route.params.jobId)
    }

    @computed get job(){
        return this._job;
    }


    @action loadJob(){
        this.jobStore.getJobInfo()
            .then(action(res => {
                this._job = res;
            }))
            .catch(action(e => {
                console.log(e);
            }))
    }

    componentDidMount(){
        this.loadJob();
    }

    handleShowImages = (evt) => {
        this.$f7router.navigate(`/jobInfo/${this.$f7route.params.jobId}/pages`, {
            props: {
                job: this.job
            }
        });
    };

    handleDownloadImages = (evt) => {
        this.jobFilesStore.getImages(this.firstPage,this.count);
    };

    handleDownloadPdf = (evt) => {
        this.jobFilesStore.getPdf();
    };

    handleDeleteJob = (evt) => {
        this.jobStore.deleteJob()
            .then(action(res => {
                this.$f7router.navigate(`/`);
            }));
    };

    handleUpdateJob = (evt) => {
        this.$f7router.navigate(`/updateJob`, {
            props: {
                job: this.job
            }
        });
    };

    handleFirstPageChange = (evt) => {
        this.firstPage = evt;
    };

    handleCountChange = (evt) => {
        this.count = evt;
    };


    render(){
        return (
            <Page>
                <AppNavbar/>
                <Loader isLoading={this.jobStore.isLoading || this.jobFilesStore.isLoading}>
                    <Block >
                        <div style={{display: "flex",justifyContent: "space-between"}}>
                            <span>Job ID:</span>
                            <span>{this.job.jobId}</span>
                        </div>
                        <div style={{display: "flex",justifyContent: "space-between"}}>
                            <span>Job status:</span>
                            <span>{this.job.status}</span>
                        </div>
                        <div style={{display: "flex",justifyContent: "space-between"}}>
                            <span>Page count:</span>
                            <span>{this.job.pageCount}</span>
                        </div>
                    </Block>
                    <div style={{display: "flex",justifyContent: "space-around"}}>
                        <div style={{display: "flex", flexDirection: "column"}}>
                            <small style={{display: "flex",justifyContent: "center"}}>First page</small>
                            <Stepper value={this.firstPage} max={1000} onStepperChange={this.handleFirstPageChange}/>
                        </div>
                        <div style={{display: "flex", flexDirection: "column"}}>
                            <small style={{display: "flex",justifyContent: "center"}}>Count</small>
                            <Stepper value={this.count} max={1000} onStepperChange={this.handleCountChange}/>
                        </div>
                    </div>
                    <List inset>
                        <ListButton title="Show pages" onClick={this.handleShowImages}/>
                        <ListButton title="Download pdf" onClick={this.handleDownloadPdf}/>
                        <ListButton title="Delete job" onClick={this.handleDeleteJob}/>
                        <ListButton title="Update job" onClick={this.handleUpdateJob}/>
                        <ListButton title="Download images" onClick={this.handleDownloadImages}/>
                    </List>
                </Loader>

            </Page>
        );
    }
}