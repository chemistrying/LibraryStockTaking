<template>
    <div class="card h-100 w-100" style="auto">
        <!-- <img src="..." class="card-img-top" alt="..."> -->
        <div class="card-body text-center">
            <h5 class="card-title"> {{ session.sessionName }} </h5>
            <p class="card-text"> {{ session.description }} </p>
            <!-- <p class="card-text"> #{{ session.id }}</p> -->
            <a href="#" class="btn btn-primary" :class="{ disabled: !session.isActive }"
                @click.prevent="jumpToSession(this.session)"> Enter </a>
        </div>
        <div class="card-footer text-body-secondary">
            Start Time: {{ this.$root.readableTime(session.startDate) }}
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
            cardCount: 0,
        }
    },
    methods: {
        jumpToSession(session) {
            if (session.isActive) {
                this.$root.activePage = this.$root.appPage.SESSION;
                this.$root.sessionId = session.id;
                console.log(session.id);
            }
        }
    }
}
</script>