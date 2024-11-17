<template>
    <div class="card h-100 w-100" style="auto">
        <!-- <img src="..." class="card-img-top" alt="..."> -->
        <div class="card-body text-center">
            <h5 class="card-title"> {{ session.sessionName }} </h5>
            <p class="card-text"> {{ session.description }} </p>
            <p class="card-text text-body-secondary" v-if="this.$root.isAdmin"> #{{ session.id }}</p>
            <a href="#" class="btn btn-primary" :class="{ disabled: !session.isActive }"
                @click.prevent="jumpToSession(this.session)"> Enter </a>
        </div>
        <div class="card-footer text-body-secondary">
            <div class="row">
                <div class="col-auto me-auto">
                    Start Time: {{ this.$root.readableTime(session.startDate) }}
                </div>
                <div class="col-auto" v-if="this.$root.isAdmin">
                    <button class="btn btn-danger pull-right" @click.prevent="deleteSession()">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash"
                            viewBox="0 0 16 16">
                            <path
                                d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0z" />
                            <path
                                d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4zM2.5 3h11V2h-11z" />
                        </svg>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ['session'],
    computed: {
    },
    data() {
        return {
            doubleConfirm: 0
        }
    },
    methods: {
        jumpToSession(session) {
            if (session.isActive) {
                this.$root.activePage = this.$root.appPage.SESSION;
                this.$root.sessionId = session.id;
                console.log(session.id);
            }
        },
        async deleteSession() {
            this.doubleConfirm++;
            if (this.doubleConfirm === 1) {
                alert("Are you sure to delete this session? This operation is irrecoverable.")
                alert("If you know what you are doing right now, click the button once again.")
            } else {
                var response = await fetch(`${this.$root.apiUrl}/api/sessions/${this.session.id}`, {
                    method: 'DELETE'
                });

                if (response.status == 204) {
                    this.$parent.failed = false;
                    this.$parent.message = "You have successfully deleted this session.";
                } else if (response.status == 404) {
                    this.$parent.failed = true;
                    this.$parent.message = "Invalid session ID.";
                } else {
                    this.$parent.failed = true;
                    this.$parent.message = "Server error. Please try again later.";
                }
            }
        }
    }
}
</script>