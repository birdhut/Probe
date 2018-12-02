export enum ProbeArgType {
    String = 0,
    Date = 1,
    Number = 2,
    DateTime = 3
}

export interface ProbeArg {
    name: string;
    type: ProbeArgType;
    isRequired: boolean;
}
export interface ProbeInfo {
    id: string;
    description: string;
    args: ProbeArg[];
}

export interface ProbeRunArgs {
    [key: string]: string;
}

export interface IProbeApiClient {
    getProbes: () => Promise<ProbeInfo[]>;
    runProbe: (id: string, args: ProbeRunArgs) => Promise<any>;
}

class HttpProbeClient implements IProbeApiClient {
    constructor(private baseUrl: string = "/api/probe") {
    }
    public getProbes(): Promise<ProbeInfo[]> {
        return fetch(this.baseUrl)
            .then((response: Response) => {
                return response.json();
            })
            .then((probes: ProbeInfo[]) => {
                return probes;
            });
    }
    public runProbe(id: string, args: ProbeRunArgs): Promise<any> {
        return fetch(`${this.baseUrl}/${id}`, {
            method: "POST",
            mode: "cors",
            cache: "no-cache",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json",
            },
            body: JSON.stringify(args)
        })
            .then((response: Response) => {
                return response.json();
            })
            .then((result: any) => {
                return result;
            });
    }
}

class MockProbeClient implements IProbeApiClient {
    private probes: ProbeInfo[] = [
        {
            id: "Test",
            description: "This is a test",
            args: [
                { name: "incidental", type: ProbeArgType.String, isRequired: false },
            ],
        },
        {
            id: "Test Required",
            description: "This is a test with required arguments",
            args: [
                { name: "the date", type: ProbeArgType.Date, isRequired: true },
                { name: "a number", type: ProbeArgType.Number, isRequired: true },
            ],
        },
        {
            id: "Test DateTime",
            description: "This is a donkey",
            args: [
                { name: "start time", type: ProbeArgType.DateTime, isRequired: true },
            ],
        },
    ];

    public async getProbes(): Promise<ProbeInfo[]> {
        await this.sleep(2000);
        return Promise.resolve(this.probes);
    }

    private sleep(ms: number) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    public async runProbe(id: string, args: ProbeRunArgs): Promise<any> {
        await this.sleep(2000);
        const probe: ProbeInfo = this.probes.find(p => p.id === id);
        if (probe != null) {
            return Promise.resolve({ probe, args });
        } else {
            return Promise.reject(`Probe ${id} does not exist`);
        }
    }
}

const client: IProbeApiClient = config.useMockData ? new MockProbeClient() : new HttpProbeClient(config.apiBaseUrl);
export default client;