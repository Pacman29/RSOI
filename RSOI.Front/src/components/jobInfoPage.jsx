import React from "react";
import PropTypes from "prop-types";
import {Block, Navbar, Page, List, ListButton, Row, Col, Stepper} from "framework7-react";
import {inject, observer} from "mobx-react";
import JobsStore from "../stores/JobsStore";
import {computed, observable, action} from "mobx";
import Loader from "./loader";
import Job from "../models/job";
import FilesStore from "../stores/FilesStore";

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
    count = 10;

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
        console.log("Show Images")
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
        console.log("Show Images")
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
                <Navbar title="Recognize Service" backLink="Back"/>
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
                            <Stepper value={this.firstPage} onStepperChange={this.handleFirstPageChange}></Stepper>
                        </div>
                        <div style={{display: "flex", flexDirection: "column"}}>
                            <small style={{display: "flex",justifyContent: "center"}}>Count</small>
                            <Stepper value={this.count} onStepperChange={this.handleCountChange}></Stepper>
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