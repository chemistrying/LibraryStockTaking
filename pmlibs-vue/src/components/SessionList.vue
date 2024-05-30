<template>
    <div class="container text-center">
        <h3> Current Acitve Session: </h3>
    </div>
    <div class="container">
        <div class="row row-cols-1 row-cols-lg-3 justify-content-lg-center g-4">
            <div class="col" v-for="session in this.$root.allSessionsInfo">
                <session-card :session="session"></session-card>
            </div>
        </div>
    </div>
</template>

<script>
import SessionCard from './SessionCard.vue';

export default {
    // props: ['page', 'isActive'],
    components: {
        SessionCard
    },
    computed: {
        async fetchAllSessions() {
            this.$root.allSessionsInfo = await fetch(`${this.$root.apiUrl}/api/sessions`).then(response => response.json());
        }
    },
    data() {
        return {
            cardCount: 0,
            allSessionsInfo: {}
        }
    },
    async mounted() {
        this.$root.allSessionsInfo = await fetch(`${this.$root.apiUrl}/api/sessions`).then(response => response.json());

        // user page
        this.$root.allSessionsInfo = this.$root.allSessionsInfo.filter(session => session.isActive);
    }
}
</script>