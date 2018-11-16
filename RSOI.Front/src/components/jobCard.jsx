import React from "react";
import PropTypes from "prop-types";
import Job, {enumJobStatus} from "../models/job";
import {inject, observer} from "mobx-react";
import {computed, observable, action} from "mobx";
import FilesStore from "../stores/FilesStore";
import {Card, CardHeader,CardContent,CardFooter,Link} from "framework7-react";
import Loader from "./loader";

@inject('filesStore')
@observer
export default class JobCard extends React.Component{
    static propTypes = {
        job: PropTypes.instanceOf(Job),
        filesStore: PropTypes.instanceOf(FilesStore)
    };

    @observable _img;

    constructor(props){
        super(props);
        this.jobFileStore = this.props.filesStore.getFileStoreForJobId(this.props.job.jobId);
    }

    componentDidMount(){
        this.loadPriview();
    }

    @action loadPriview(){
        if(this.props.job.status === enumJobStatus.DONE){
            this.jobFileStore.getImage()
                .then(action(res => {
                    this._img = URL.createObjectURL(res);
                }))
                .catch(action(err => console.log(err)));
        }
    }

    @computed get img(){
        return this._img
    }

    @action handlerOnCardClick = (evt) => {
        this.$f7router.navigate(`/jobInfo/${this.job.jobId}`);
    };

    render(){
        switch (this.props.job.status) {
            case enumJobStatus.DONE:
                return (
                    <Card onClick={this.handlerOnCardClick}>
                        <Loader isLoading={this.jobFileStore.isLoading || this._img === undefined}>
                            <CardHeader>
                                <img style={{maxWidth: "100%"}} src={this.img}/>
                            </CardHeader>
                            <CardContent>
                                {this.props.job.jobId}
                            </CardContent>
                            <CardFooter>
                                {this.props.job.status}
                                <Link href={`/jobInfo/${this.props.job.jobId}`}>
                                    Show more
                                </Link>
                            </CardFooter>
                        </Loader>
                    </Card>
                );
            default:
                return (
                    <Card title={this.props.job.jobId} footer={this.props.job.status}/>
                );
        }
    }
}