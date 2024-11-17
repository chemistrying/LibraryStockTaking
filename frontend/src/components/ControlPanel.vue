<template>
    <div v-if="message !== ''" :class="failureClass" role="alert">
        {{ message }}
    </div>

    
    <div class="row row-cols-1 g-2">
        <!-- List All Session (Plug Session List) -->
        <div class="col" v-if="this.$root.isAdmin">
            <session-list></session-list>
        </div>
        <div class="col">
            <credentials-card class="mt-3"></credentials-card>
        </div>
        <div class="col">
            <logout-card></logout-card>
        </div>
        <!-- Admin Cards -->
        <div class="col" v-if="this.$root.isAdmin">
            <!-- Select Active Session -->
            <active-card></active-card>
        </div>
        <div class="col" v-if="this.$root.isAdmin">
            <!-- Create Session -->
            <create-session-card></create-session-card>
        </div>
        <!-- Modify Session Metadata -->
        <div class="col" v-if="this.$root.isAdmin">
            <change-session-card></change-session-card>
        </div>
        <!-- Reload Booklist -->
        <div class="col" v-if="this.$root.isAdmin">
            <reload-booklist-card></reload-booklist-card>
        </div>
        <!-- Modify Bookshelf Description (not here though) -->
    </div>
</template>

<script>
import CredentialsCard from './CredentialsCard.vue';
import SessionList from './SessionList.vue';
import ActiveCard from './ActiveCard.vue';
import CreateSessionCard from './CreateSessionCard.vue';
import ChangeSessionCard from './ChangeSessionCard.vue';
import ReloadBooklistCard from './ReloadBooklistCard.vue';
import LogoutCard from './LogoutCard.vue';

export default {
    props: ['isAdmin'],
    components: {
        CredentialsCard, SessionList, ActiveCard, CreateSessionCard, ChangeSessionCard, ReloadBooklistCard, LogoutCard
    },
    computed: {
        failureClass() {
            return {
                'alert': true,
                'alert-success': !this.failed,
                'alert-danger': this.failed
            }
        }
    },
    data() {
        return {
            failed: false,
            message: ''
        }
    }
}
</script>