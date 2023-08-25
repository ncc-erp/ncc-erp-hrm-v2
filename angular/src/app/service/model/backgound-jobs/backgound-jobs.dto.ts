export class BackgoundJobsDto{
    id: number;
    jobType: string;
    jobArgs: string;
    tryCount: number;
    lastTryTime: string;
    nextTryTime: string;
    isAbandoned: boolean;
    priority: number;
    creationTime: string;
    creatorUserId: number;
    description: string;
    subJobType: string;
}
